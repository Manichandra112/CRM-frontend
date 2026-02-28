using CRM_Backend.Data;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Exceptions;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CRM_Backend.Domain.Enums;

namespace CRM_Backend.Services.Implementations;

public class AdminUserDetailsService : IAdminUserDetailsService
{
    private readonly CrmAuthDbContext _context;

    public AdminUserDetailsService(CrmAuthDbContext context)
    {
        _context = context
            ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserDetailsDto> GetUserDetailsAsync(long userId)
    {
        if (userId <= 0)
            throw new ValidationException("Invalid user id.");

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId && u.DeletedAt == null)
            .Select(u => new UserDetailsDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                AccountStatus = u.AccountStatus.ToString(),
                LockReason = u.LockReason,

                Profile = new AdminUserProfileDto
                {
                    FirstName = u.Profile.FirstName,
                    LastName = u.Profile.LastName,
                    MobileNumber = u.Profile.MobileNumber,
                    Gender = u.Profile.Gender,

                    AddressLine1 = u.Profile.AddressLine1,
                    City = u.Profile.City,
                    State = u.Profile.State,
                    Country = u.Profile.Country,
                    PostalCode = u.Profile.PostalCode,

                    LanguagePreference = u.Profile.LanguagePreference,
                    Timezone = u.Profile.Timezone
                },

                Organization = new AdminUserOrganizationDto
                {
                    DomainName = u.Domain.DomainName,
                    Department = u.Department,
                    Designation = u.Designation,
                    ManagerName = u.Manager != null
                        ? u.Manager.Profile.FirstName + " " + u.Manager.Profile.LastName
                        : null,
                    AssignedRegion = u.AssignedRegion,
                    AssignedBranch = u.AssignedBranch,
                    EmploymentType = u.EmploymentType,
                    WorkShift = u.WorkShift,
                    Remarks = u.Remarks,
                    AccessStartDate = u.AccessStartDate,
                    AccessEndDate = u.AccessEndDate,
                },

                Roles = u.UserRoles
                    .Select(r => r.Role.RoleName)
                    .ToList(),

                CreatedAt = u.CreatedAt,
                LastActivityAt = u.LastActivityAt
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException($"User {userId} not found.");

        return user;
    }
}
