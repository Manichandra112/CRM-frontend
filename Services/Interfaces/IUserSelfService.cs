using CRM_Backend.DTOs.Self;
using CRM_Backend.DTOs.Users;

public interface IUserSelfService
{
    Task<SelfProfileDto> GetProfileAsync(long userId);
    Task UpdateProfileAsync(long userId, UpdateUserProfileDto dto);
    Task<List<SelfTeamMemberDto>> GetTeamAsync(long managerId);
    Task<SelfSecurityOverviewDto> GetSecurityOverviewAsync(long userId);
}
