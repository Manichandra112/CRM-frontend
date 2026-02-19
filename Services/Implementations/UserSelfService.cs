using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Self;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.Implementations
{
    public class UserSelfService : IUserSelfService
    {
        private readonly CrmAuthDbContext _context;

        public UserSelfService(CrmAuthDbContext context)
        {
            _context = context;
        }

        public async Task<SelfProfileDto> GetProfileAsync(long userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Profile)
                .Include(u => u.Domain)
                .Include(u => u.Manager)
                    .ThenInclude(m => m.Profile)
                .FirstOrDefaultAsync(u => u.UserId == userId)
                ?? throw new NotFoundException("User not found.");

            return new SelfProfileDto
            {
                Identity = new IdentityInfo
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    EmployeeId = user.EmployeeId
                },

                Organization = new OrganizationInfo
                {
                    Domain = user.Domain?.DomainCode,
                    Department = user.Department,
                    Designation = user.Designation,
                    EmploymentType = user.EmploymentType,
                    WorkShift = user.WorkShift
                },

                Personal = new PersonalInfo
                {
                    FirstName = user.Profile?.FirstName,
                    LastName = user.Profile?.LastName,
                    Gender = user.Profile?.Gender,
                    MobileNumber = user.Profile?.MobileNumber,
                    AddressLine1 = user.Profile?.AddressLine1,
                    City = user.Profile?.City,
                    State = user.Profile?.State,
                    Country = user.Profile?.Country,
                    PostalCode = user.Profile?.PostalCode
                },

                Preferences = new PreferenceInfo
                {
                    Language = user.Profile?.LanguagePreference,
                    Timezone = user.Profile?.Timezone
                },

                Manager = user.Manager == null
                    ? null
                    : new ManagerInfo
                    {
                        Id = user.Manager.UserId,
                        Name = user.Manager.Profile != null
                            ? $"{user.Manager.Profile.FirstName} {user.Manager.Profile.LastName}"
                            : user.Manager.Username
                    },

                Account = new AccountInfo
                {
                    AccountStatus = user.AccountStatus,
                    CreatedAt = user.CreatedAt,
                    AccessStartDate = user.AccessStartDate,
                    AccessEndDate = user.AccessEndDate
                }
            };
        }


        public async Task UpdateProfileAsync(long userId, UpdateSelfProfileDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserId == userId)
                ?? throw new NotFoundException("User not found.");

            // Ensure profile exists
            if (user.Profile == null)
            {
                user.Profile = new Domain.Entities.UserProfile
                {
                    UserId = userId
                };
            }

            bool changed = false;

            // -------- PATCH STYLE UPDATES --------

            if (dto.FirstName != null && dto.FirstName != user.Profile.FirstName)
            {
                user.Profile.FirstName = dto.FirstName;
                changed = true;
            }

            if (dto.LastName != null && dto.LastName != user.Profile.LastName)
            {
                user.Profile.LastName = dto.LastName;
                changed = true;
            }

            if (dto.Gender != null && dto.Gender != user.Profile.Gender)
            {
                user.Profile.Gender = dto.Gender;
                changed = true;
            }

            if (dto.MobileNumber != null && dto.MobileNumber != user.Profile.MobileNumber)
            {
                user.Profile.MobileNumber = dto.MobileNumber;
                changed = true;
            }

            if (dto.AddressLine1 != null && dto.AddressLine1 != user.Profile.AddressLine1)
            {
                user.Profile.AddressLine1 = dto.AddressLine1;
                changed = true;
            }

            if (dto.City != null && dto.City != user.Profile.City)
            {
                user.Profile.City = dto.City;
                changed = true;
            }

            if (dto.State != null && dto.State != user.Profile.State)
            {
                user.Profile.State = dto.State;
                changed = true;
            }

            if (dto.Country != null && dto.Country != user.Profile.Country)
            {
                user.Profile.Country = dto.Country;
                changed = true;
            }

            if (dto.PostalCode != null && dto.PostalCode != user.Profile.PostalCode)
            {
                user.Profile.PostalCode = dto.PostalCode;
                changed = true;
            }

            if (dto.LanguagePreference != null && dto.LanguagePreference != user.Profile.LanguagePreference)
            {
                user.Profile.LanguagePreference = dto.LanguagePreference;
                changed = true;
            }

            if (dto.Timezone != null && dto.Timezone != user.Profile.Timezone)
            {
                user.Profile.Timezone = dto.Timezone;
                changed = true;
            }

            if (changed)
            {
                user.UpdatedAt = DateTime.UtcNow;

                _context.AuditLogs.Add(new AuditLog
                {
                    ActorUserId = userId,
                    TargetUserId = userId,
                    Action = "SELF_PROFILE_UPDATE",
                    Module = "USERS",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<SelfTeamMemberDto>> GetTeamAsync(long managerId)
        {
            return await _context.Users
                .Where(u => u.ManagerId == managerId)
                .Select(u => new SelfTeamMemberDto
                {
                    UserId = u.UserId,
                    Name = u.Profile != null
                        ? (u.Profile.FirstName ?? "") + " " + (u.Profile.LastName ?? "")
                        : u.Username,
                    Designation = u.Designation
                })
                .ToListAsync();
        }


        public async Task<SelfSecurityOverviewDto> GetSecurityOverviewAsync(long userId)
        {
            var security = await _context.UserSecurity
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (security == null)
            {
                return new SelfSecurityOverviewDto
                {
                    MfaEnabled = false,
                    MfaType = null,
                    LastLoginAt = null,
                    LastLoginIp = null,
                    LastLoginDevice = null,
                    FailedLoginCount = 0,
                    PasswordLastChangedAt = null
                };
            }

            return new SelfSecurityOverviewDto
            {
                MfaEnabled = security.MfaEnabled,
                MfaType = security.MfaType,
                LastLoginAt = security.LastLoginAt,
                LastLoginIp = security.LastLoginIp,
                LastLoginDevice = security.LastLoginDevice,
                FailedLoginCount = security.FailedLoginCount,
                PasswordLastChangedAt = security.PasswordLastChangedAt
            };
        }
    }
}
