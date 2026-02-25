using CRM_Backend.DTOs.Users;

namespace CRM_Backend.Services.Interfaces;
using CRM_Backend.Domain.Enums;
public interface IUserManagementService
{
    

    Task<long> CreateUserAsync(CreateUserDto dto, long createdBy);
    //Task UpdateUserAsync(long userId, UpdateUserDto dto, long updatedBy);

    Task AssignManagerAsync(long userId, long managerId, long performedBy);
    Task<List<UserLookupDto>> GetUsersAsync();
    Task<List<UserLookupDto>> GetUsersByRoleAsync(string roleCode);
    Task<List<UserLookupDto>> GetEmployeesByDomainAsync(string domainCode);
    Task<List<UserLookupDto>> GetManagersByDomainAsync(string domainCode);
    Task<List<UserLookupDto>> GetAllManagersAsync();
    Task<object> GetUserDetailsAsync(long userId);
    Task<List<UserLookupDto>> GetTeamByManagerAsync(long managerId);
    Task AssignRoleToUserAsync(long userId, string roleCode, long assignedBy);
    Task RemoveRoleFromUserAsync(long userId, string roleCode);
    Task<List<UserRoleDto>> GetUserRolesAsync(long userId);
    Task LockUserAsync(long userId, string reason, long lockedBy);
    Task UnlockUserAsync(long userId, long unlockedBy);
    Task UpdateSelfProfileAsync(long userId, UpdateUserProfileDto dto);
    
    Task<List<UserListDto>> GetAdminUsersByDomainAsync(
        string domainCode,
        string? search,
        string? status,
        string? roleCode
    );
    Task UpdateUserStatusAsync(
    long userId,
    AccountStatus newStatus,
    long updatedBy);


    Task UpdateUserOrganizationAsync(long userId, UpdateUserOrganizationDto dto, long actorId);
    Task UpdateUserProfileAsync(long userId, UpdateUserProfileDto dto, long actorId);

    Task UpdateUserEmailAsync(long userId, string newEmail, long updatedBy);
    Task UpdateUsernameAsync(long userId, string newUsername, long updatedBy);
    Task UpdateUserDomainAsync(long userId, string domainCode, long updatedBy);
}