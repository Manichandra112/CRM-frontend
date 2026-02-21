using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace CRM_Backend.Services.Implementations;

public class UserManagementService : IUserManagementService
{
    private readonly CrmAuthDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IRoleRepository _roles;
    private readonly IDomainRepository _domains;
    private readonly IUserRepository _users;
    private readonly IAuditLogService _audit;

    public UserManagementService(
        CrmAuthDbContext context,
        IPasswordService passwordService,
        IRoleRepository roles,
        IDomainRepository domains,
        IUserRepository users,
        IAuditLogService audit)
    {
        _context = context;
        _passwordService = passwordService;
        _roles = roles;
        _domains = domains;
        _users = users;
        _audit = audit;
    }

    public async Task<long> CreateUserAsync(CreateUserDto dto, long createdBy)
    {
        if (dto == null)
            throw new ValidationException("User data is required.");

        if (dto.Profile == null)
            throw new ValidationException("Profile information is required.");

        if (dto.RoleCodes == null || !dto.RoleCodes.Any())
            throw new ValidationException("At least one role must be assigned.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var domain = await _domains.GetByCodeAsync(dto.DomainCode)
                ?? throw new NotFoundException($"Domain '{dto.DomainCode}' not found.");

            var userExists = await _context.Users.AnyAsync(u =>
                u.Email == dto.Email || u.Username == dto.Username);

            if (userExists)
                throw new ConflictException("A user with the same email or username already exists.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                AccountStatus = "ACTIVE",
                DomainId = domain.DomainId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                CreatedVia = "ADMIN"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Needed to generate UserId

            _context.UserProfiles.Add(new UserProfile
            {
                UserId = user.UserId,
                FirstName = dto.Profile.FirstName,
                LastName = dto.Profile.LastName,
                MobileNumber = dto.Profile.MobileNumber
            });

            _context.UserSecurity.Add(new UserSecurity
            {
                UserId = user.UserId,
                ForcePasswordReset = true,
                FailedLoginCount = 0,
                MfaEnabled = false
            });

            _context.UserPasswords.Add(new UserPassword
            {
                UserId = user.UserId,
                PasswordHash = _passwordService.HashPassword(dto.TemporaryPassword),
                IsCurrent = true,
                CreatedAt = DateTime.UtcNow
            });

            foreach (var roleCode in dto.RoleCodes)
            {
                var roleId = await _roles.GetRoleIdByCodeAsync(roleCode);

                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = createdBy
                });
            }

            _context.AuditLogs.Add(new AuditLog
            {
                ActorUserId = createdBy,
                TargetUserId = user.UserId,
                Action = "USER_CREATE",
                Module = "USERS",
                Metadata = JsonSerializer.Serialize(new
                {
                    domain = dto.DomainCode,
                    roles = dto.RoleCodes
                }),
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return user.UserId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }



    public async Task UpdateSelfProfileAsync(long userId, UpdateSelfProfileDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.Profile == null)
        {
            user.Profile = new UserProfile
            {
                UserId = userId
            };
        }

        user.Profile.FirstName = dto.FirstName;
        user.Profile.LastName = dto.LastName;
        user.Profile.Gender = dto.Gender;
        user.Profile.MobileNumber = dto.MobileNumber;
        user.Profile.AddressLine1 = dto.AddressLine1;
        user.Profile.City = dto.City;
        user.Profile.State = dto.State;
        user.Profile.Country = dto.Country;
        user.Profile.PostalCode = dto.PostalCode;
        user.Profile.LanguagePreference = dto.LanguagePreference;
        user.Profile.Timezone = dto.Timezone;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }



    // ✅ SINGLE, CORRECT METHOD — NO DUPLICATES
    public async Task AssignManagerAsync(long userId, long managerId, long performedBy)
    {
        if (userId == managerId)
            throw new BusinessRuleException("A user cannot be assigned as their own manager.");

        var user = await _context.Users.FindAsync(userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        var manager = await _context.Users.FindAsync(managerId)
            ?? throw new NotFoundException($"Manager {managerId} not found.");

        user.ManagerId = managerId;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = performedBy,   // ✅ Correct actor
            TargetUserId = userId,
            Action = "ASSIGN_MANAGER",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                managerId = managerId
            }),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }


    public async Task<List<UserLookupDto>> GetUsersByRoleAsync(string roleCode)
    {
        return await _context.UserRoles
            .Where(ur => ur.Role.RoleCode == roleCode)
            .Select(ur => new UserLookupDto
            {
                UserId = ur.UserId,
                Name = ur.User.Profile.FirstName + " " + ur.User.Profile.LastName
            })
            .ToListAsync();
    }

    public async Task<List<UserLookupDto>> GetUsersAsync()
    {
        return await _context.Users
            .Where(u => u.AccountStatus == "ACTIVE")
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }
    // --------------------------------------------------
    // 🔹 GET EMPLOYEES BY DOMAIN
    // --------------------------------------------------
    public async Task<List<UserLookupDto>> GetEmployeesByDomainAsync(string domainCode)
    {
        return await _context.Users
            .Where(u =>
                u.AccountStatus == "ACTIVE" &&
                u.Domain.DomainCode == domainCode
            )
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName
            })
            .ToListAsync();
    }

    // --------------------------------------------------
    // 🔹 GET MANAGERS BY DOMAIN (permission-based)
    // --------------------------------------------------
    public async Task<List<UserLookupDto>> GetManagersByDomainAsync(string? domainCode)
    {
        var query = _context.Users
            .Where(u =>
                u.AccountStatus == "ACTIVE" &&
                u.UserRoles.Any(ur => ur.Role.RoleCode.EndsWith("_MANAGER"))
            );

        if (!string.IsNullOrWhiteSpace(domainCode))
        {
            var domain = await _domains.GetByCodeAsync(domainCode)
                ?? throw new NotFoundException($"Domain '{domainCode}' not found.");

            query = query.Where(u => u.DomainId == domain.DomainId);
        }

        return await query
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName,
                DomainCode = u.Domain.DomainCode   // 🔥 ADD THIS
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }





    // --------------------------------------------------
    // 🔹 GET SINGLE USER DETAILS (View/Edit)
    // --------------------------------------------------
    public async Task<object> GetUserDetailsAsync(long userId)
    {
        var user = await _context.Users
            .Where(u => u.UserId == userId)
            .Select(u => new
            {
                u.UserId,
                u.Username,
                u.Email,
                u.Department,
                u.Designation,
                u.AccountStatus,

                ManagerId = u.ManagerId,

                ManagerName = u.Manager != null
                    ? u.Manager.Profile.FirstName + " " + u.Manager.Profile.LastName
                    : null,

                Profile = new
                {
                    u.Profile.FirstName,
                    u.Profile.LastName,
                    u.Profile.MobileNumber
                }
            })
            .FirstOrDefaultAsync();

        return user ?? throw new NotFoundException($"User {userId} not found.");
    }


    // --------------------------------------------------
    // 🔹 GET TEAM BY MANAGER (Zoho “My Team”)
    // --------------------------------------------------
    public async Task<List<UserLookupDto>> GetTeamByManagerAsync(long managerId)
    {
        return await _context.Users
            .Where(u => u.ManagerId == managerId)
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName
            })
            .ToListAsync();
    }

    public async Task AssignRoleToUserAsync(long userId, string roleCode, long assignedBy)
    {
        var user = await _context.Users.FindAsync(userId)
?? throw new NotFoundException($"User {userId} not found.");

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleCode == roleCode)
?? throw new NotFoundException($"Role '{roleCode}' not found.");

        var alreadyAssigned = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);

        if (alreadyAssigned)
            throw new ConflictException("Role is already assigned to the user.");

        _context.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = role.RoleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        });

        await _context.SaveChangesAsync();
    }
    public async Task RemoveRoleFromUserAsync(long userId, string roleCode)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleCode == roleCode)
?? throw new NotFoundException($"Role '{roleCode}' not found.");

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId)
?? throw new BusinessRuleException("The role is not assigned to this user.");

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserRoleDto>> GetUserRolesAsync(long userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => new UserRoleDto
            {
                RoleCode = ur.Role.RoleCode,
                RoleName = ur.Role.RoleName
            })
            .ToListAsync();
    }
    //public async Task UpdateUserAsync(long userId, UpdateUserDto dto, long updatedBy)
    //{
    //    var user = await _context.Users
    //        .Include(u => u.Profile)
    //        .FirstOrDefaultAsync(u => u.UserId == userId)
    //        ?? throw new NotFoundException($"User {userId} not found.");


    //    // Track what changed (important for audit)
    //    var changes = new List<string>();

    //    // ---------------- USER FIELDS ----------------
    //    if (dto.Department != null && dto.Department != user.Department)
    //    {
    //        changes.Add($"Department: {user.Department} → {dto.Department}");
    //        user.Department = dto.Department;
    //    }

    //    if (dto.Designation != null && dto.Designation != user.Designation)
    //    {
    //        changes.Add($"Designation: {user.Designation} → {dto.Designation}");
    //        user.Designation = dto.Designation;
    //    }

    //    if (dto.EmploymentType != null && dto.EmploymentType != user.EmploymentType)
    //    {
    //        changes.Add($"EmploymentType: {user.EmploymentType} → {dto.EmploymentType}");
    //        user.EmploymentType = dto.EmploymentType;
    //    }

    //    if (dto.WorkShift != null && dto.WorkShift != user.WorkShift)
    //    {
    //        changes.Add($"WorkShift: {user.WorkShift} → {dto.WorkShift}");
    //        user.WorkShift = dto.WorkShift;
    //    }

    //    if (dto.AssignedRegion != null && dto.AssignedRegion != user.AssignedRegion)
    //    {
    //        changes.Add($"AssignedRegion: {user.AssignedRegion} → {dto.AssignedRegion}");
    //        user.AssignedRegion = dto.AssignedRegion;
    //    }

    //    if (dto.AssignedBranch != null && dto.AssignedBranch != user.AssignedBranch)
    //    {
    //        changes.Add($"AssignedBranch: {user.AssignedBranch} → {dto.AssignedBranch}");
    //        user.AssignedBranch = dto.AssignedBranch;
    //    }

    //    if (dto.Remarks != null && dto.Remarks != user.Remarks)
    //    {
    //        changes.Add("Remarks updated");
    //        user.Remarks = dto.Remarks;
    //    }

    //    // ---------------- PROFILE FIELDS ----------------
    //    if (dto.FirstName != null && dto.FirstName != user.Profile.FirstName)
    //    {
    //        changes.Add($"FirstName: {user.Profile.FirstName} → {dto.FirstName}");
    //        user.Profile.FirstName = dto.FirstName;
    //    }

    //    if (dto.LastName != null && dto.LastName != user.Profile.LastName)
    //    {
    //        changes.Add($"LastName: {user.Profile.LastName} → {dto.LastName}");
    //        user.Profile.LastName = dto.LastName;
    //    }

    //    if (dto.MobileNumber != null && dto.MobileNumber != user.Profile.MobileNumber)
    //    {
    //        changes.Add($"MobileNumber updated");
    //        user.Profile.MobileNumber = dto.MobileNumber;
    //    }

    //    user.UpdatedAt = DateTime.UtcNow;
    //    user.UpdatedBy = updatedBy;

    //    // ---------------- AUDIT LOG ----------------
    //    if (changes.Any())
    //    {
    //        _context.AuditLogs.Add(new AuditLog
    //        {
    //            ActorUserId = updatedBy,
    //            TargetUserId = userId,
    //            Action = "USER_UPDATE",
    //            Module = "USERS",
    //            Metadata = JsonSerializer.Serialize(new
    //            {
    //                changes = changes
    //            }),

    //            CreatedAt = DateTime.UtcNow
    //        });
    //    }

    //    await _context.SaveChangesAsync();
    //}



    public async Task LockUserAsync(long userId, string reason, long lockedBy)
    {
        var user = await _context.Users.FindAsync(userId)
?? throw new NotFoundException($"User {userId} not found.");

        if (user.AccountStatus == "LOCKED")
            throw new BusinessRuleException("User account is already locked.");

        user.AccountStatus = "LOCKED";
        user.LockReason = reason;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = lockedBy;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = lockedBy,
            TargetUserId = userId,
            Action = "USER_LOCK",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                reason = reason,
                lockedBy = lockedBy,
                lockedAt = DateTime.UtcNow
            }),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task UnlockUserAsync(long userId, long unlockedBy)
    {
        var user = await _context.Users.FindAsync(userId)
?? throw new NotFoundException($"User {userId} not found.");

        if (user.AccountStatus != "LOCKED")
            throw new BusinessRuleException("User account is not locked.");

        user.AccountStatus = "ACTIVE";
        user.LockReason = null;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = unlockedBy;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = unlockedBy,
            TargetUserId = userId,
            Action = "USER_UNLOCK",
            Module = "USERS",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }



    public async Task<List<UserListDto>> GetAdminUsersByDomainAsync(
       string domainCode,
       string? search,
       string? status,
       string? roleCode)
    {
        var query = _context.Users
            .Where(u =>
                u.Domain.DomainCode == domainCode &&
                u.DeletedAt == null
            )
            .AsQueryable();

        // 🔍 SEARCH (name, email, department)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();

            query = query.Where(u =>
                u.Username.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term) ||
                u.Profile.FirstName.ToLower().Contains(term) ||
                u.Profile.LastName.ToLower().Contains(term) ||
                (u.Department != null && u.Department.ToLower().Contains(term))
            );
        }

        // 🔒 STATUS FILTER
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(u => u.AccountStatus == status);
        }

        // 🎭 ROLE FILTER
        if (!string.IsNullOrWhiteSpace(roleCode))
        {
            query = query.Where(u =>
                u.UserRoles.Any(ur => ur.Role.RoleCode == roleCode)
            );
        }

        return await query
            .Select(u => new UserListDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Department = u.Department,
                Designation = u.Designation,
                AccountStatus = u.AccountStatus,
                AssignedBranch = u.AssignedBranch,

                ManagerName = u.ManagerId != null
                    ? _context.Users
                        .Where(m => m.UserId == u.ManagerId)
                        .Select(m => m.Profile.FirstName + " " + m.Profile.LastName)
                        .FirstOrDefault()
                    : null,

                Roles = u.UserRoles
                    .Select(ur => ur.Role.RoleCode)
                    .ToList()
            })
            .OrderBy(u => u.Username)
            .ToListAsync();
    }
    public async Task<List<UserLookupDto>> GetAllManagersAsync()
    {
        return await _context.Users
            .Where(u =>
                u.AccountStatus == "ACTIVE" &&
                u.UserRoles.Any(ur => ur.Role.RoleCode.EndsWith("_MANAGER"))
            )
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName,
                DomainCode = u.Domain.DomainCode
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task UpdateUserOrganizationAsync(
    long userId,
    UpdateUserOrganizationDto dto,
    long updatedBy)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        var changes = new List<string>();

        if (dto.Department != null && dto.Department != user.Department)
        {
            changes.Add($"Department: {user.Department} → {dto.Department}");
            user.Department = dto.Department;
        }

        if (dto.Designation != null && dto.Designation != user.Designation)
        {
            changes.Add($"Designation: {user.Designation} → {dto.Designation}");
            user.Designation = dto.Designation;
        }

        if (dto.EmploymentType != null && dto.EmploymentType != user.EmploymentType)
        {
            changes.Add($"EmploymentType: {user.EmploymentType} → {dto.EmploymentType}");
            user.EmploymentType = dto.EmploymentType;
        }

        if (dto.WorkShift != null && dto.WorkShift != user.WorkShift)
        {
            changes.Add($"WorkShift: {user.WorkShift} → {dto.WorkShift}");
            user.WorkShift = dto.WorkShift;
        }

        if (dto.AssignedRegion != null && dto.AssignedRegion != user.AssignedRegion)
        {
            changes.Add($"AssignedRegion: {user.AssignedRegion} → {dto.AssignedRegion}");
            user.AssignedRegion = dto.AssignedRegion;
        }

        if (dto.AssignedBranch != null && dto.AssignedBranch != user.AssignedBranch)
        {
            changes.Add($"AssignedBranch: {user.AssignedBranch} → {dto.AssignedBranch}");
            user.AssignedBranch = dto.AssignedBranch;
        }

        if (dto.Remarks != null && dto.Remarks != user.Remarks)
        {
            changes.Add("Remarks updated");
            user.Remarks = dto.Remarks;
        }


        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        if (changes.Any())
        {
            _context.AuditLogs.Add(new AuditLog
            {
                ActorUserId = updatedBy,
                TargetUserId = userId,
                Action = "USER_UPDATE_ORGANIZATION",
                Module = "USERS",
                Metadata = JsonSerializer.Serialize(new { changes }),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }
    public async Task UpdateUserProfileAsync(
    long userId,
    UpdateUserProfileByAdminDto dto,
    long updatedBy)
    {
        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        if (user.Profile == null)
        {
            user.Profile = new UserProfile
            {
                UserId = userId
            };
        }

        var changes = new List<string>();

        if (dto.FirstName != null &&
            dto.FirstName != user.Profile.FirstName)
        {
            changes.Add($"FirstName: {user.Profile.FirstName} → {dto.FirstName}");
            user.Profile.FirstName = dto.FirstName;
        }

        if (dto.LastName != null &&
            dto.LastName != user.Profile.LastName)
        {
            changes.Add($"LastName: {user.Profile.LastName} → {dto.LastName}");
            user.Profile.LastName = dto.LastName;
        }

        if (dto.Gender != null &&
            dto.Gender != user.Profile.Gender)
        {
            changes.Add($"Gender updated");
            user.Profile.Gender = dto.Gender;
        }

        if (dto.MobileNumber != null &&
            dto.MobileNumber != user.Profile.MobileNumber)
        {
            changes.Add("MobileNumber updated");
            user.Profile.MobileNumber = dto.MobileNumber;
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        if (changes.Any())
        {
            _context.AuditLogs.Add(new AuditLog
            {
                ActorUserId = updatedBy,
                TargetUserId = userId,
                Action = "USER_UPDATE_PROFILE",
                Module = "USERS",
                Metadata = JsonSerializer.Serialize(new { changes }),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserEmailAsync(
    long userId,
    string newEmail,
    long updatedBy)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ValidationException("Email is required.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        if (user.Email == newEmail)
            return;

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == newEmail && u.UserId != userId);

        if (emailExists)
            throw new ConflictException("Email is already in use.");

        var oldEmail = user.Email;
        user.Email = newEmail;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        // Invalidate refresh tokens
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
            token.RevokedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = updatedBy,
            TargetUserId = userId,
            Action = "USER_UPDATE_EMAIL",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                oldEmail,
                newEmail
            }),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task UpdateUsernameAsync(
    long userId,
    string newUsername,
    long updatedBy)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ValidationException("Username is required.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        if (user.Username == newUsername)
            return;

        var exists = await _context.Users
            .AnyAsync(u => u.Username == newUsername && u.UserId != userId);

        if (exists)
            throw new ConflictException("Username already exists.");

        var oldUsername = user.Username;
        user.Username = newUsername;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = updatedBy,
            TargetUserId = userId,
            Action = "USER_UPDATE_USERNAME",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                oldUsername,
                newUsername
            }),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserDomainAsync(
    long userId,
    string domainCode,
    long updatedBy)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        var domain = await _domains.GetByCodeAsync(domainCode)
            ?? throw new NotFoundException($"Domain '{domainCode}' not found.");

        if (user.DomainId == domain.DomainId)
            return;

        var oldDomainId = user.DomainId;

        // Remove all roles
        _context.UserRoles.RemoveRange(user.UserRoles);

        user.DomainId = domain.DomainId;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = updatedBy,
            TargetUserId = userId,
            Action = "USER_UPDATE_DOMAIN",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                oldDomainId,
                newDomainId = domain.DomainId,
                rolesCleared = true
            }),
            CreatedAt = DateTime.UtcNow
        });
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserStatusAsync(
    long userId,
    string newStatus,
    long updatedBy)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ValidationException("Status is required.");

        var allowedStatuses = new[]
        {
        Domain.Constants.AccountStatus.ACTIVE,
        Domain.Constants.AccountStatus.INACTIVE,
        Domain.Constants.AccountStatus.EXITED
    };

        if (!allowedStatuses.Contains(newStatus))
            throw new ValidationException("Invalid account status.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        if (user.AccountStatus == "LOCKED")
            throw new BusinessRuleException("Locked users must be unlocked first.");

        if (user.AccountStatus == newStatus)
            return;

        var oldStatus = user.AccountStatus;

        user.AccountStatus = newStatus;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = updatedBy,
            TargetUserId = userId,
            Action = "USER_UPDATE_STATUS",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                oldStatus,
                newStatus
            }),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

}




