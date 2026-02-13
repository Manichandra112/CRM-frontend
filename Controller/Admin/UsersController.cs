//using CRM_Backend.DTOs.Users;
//using CRM_Backend.Security.Authorization;
//using CRM_Backend.Security.Extensions;
//using CRM_Backend.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CRM_Backend.Controllers;

//[ApiController]
//[Route("api/users")]

//[Authorize(Policy = "ACCOUNT_ACTIVE")]
//[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

//public class UsersController : ControllerBase
//{
//    private readonly IUserManagementService _users;

//    public UsersController(IUserManagementService users)
//    {
//        _users = users;
//    }

//    // --------------------------------------------------
//    // CREATE USER (ADMIN)
//    // --------------------------------------------------
//    [HttpPost]
//    [HasPermission("USER_CREATE")]
//    public async Task<IActionResult> CreateUser(CreateUserDto dto)
//    {
//        var actorId = User.GetUserId();
//        var userId = await _users.CreateUserAsync(dto, actorId);
//        return Ok(new { userId });
//    }

//    // --------------------------------------------------
//    // UPDATE USER (ADMIN)
//    // --------------------------------------------------
//    [HttpPut("{userId:long}")]
//    [HasPermission("USER_UPDATE")]
//    public async Task<IActionResult> UpdateUser(long userId, UpdateUserDto dto)
//    {
//        var actorId = User.GetUserId();
//        await _users.UpdateUserAsync(userId, dto, actorId);
//        return Ok();
//    }

//    // --------------------------------------------------
//    // ASSIGN MANAGER (ADMIN)
//    // --------------------------------------------------
//    [HttpPut("{userId:long}/manager")]
//    [HasPermission("USER_ASSIGN_MANAGER")]
//    public async Task<IActionResult> AssignManager(long userId, AssignManagerDto dto)
//    {
//        await _users.AssignManagerAsync(userId, dto.ManagerId);
//        return Ok();
//    }

//    // --------------------------------------------------
//    // ADMIN USER LIST (GLOBAL)
//    // --------------------------------------------------
//    [HttpGet]
//    [HasPermission("USER_VIEW_ALL")]
//    public async Task<IActionResult> GetAdminUserList(
//        [FromQuery] string domainCode,
//        [FromQuery] string? search,
//        [FromQuery] string? status,
//        [FromQuery] string? roleCode)
//    {
//        var users = await _users.GetAdminUsersByDomainAsync(
//            domainCode, search, status, roleCode);

//        return Ok(users);
//    }

//    // --------------------------------------------------
//    // MANAGERS DROPDOWN (ADMIN)
//    // --------------------------------------------------
//    [HttpGet("managers")]
//    [HasPermission("USER_VIEW_ALL")]
//    public async Task<IActionResult> GetManagers([FromQuery] string domainCode)
//    {
//        var managers = await _users.GetManagersByDomainAsync(domainCode);
//        return Ok(managers);
//    }

//    // --------------------------------------------------
//    // GLOBAL MANAGERS (ADMIN)
//    // --------------------------------------------------
//    [HttpGet("admin/managers")]
//    [HasPermission("USER_VIEW_ALL")]
//    public async Task<IActionResult> GetAllManagers()
//    {
//        return Ok(await _users.GetAllManagersAsync());
//    }

//    // --------------------------------------------------
//    // GET USER DETAILS (ADMIN VIEW)
//    // --------------------------------------------------
//    [HttpGet("{userId:long}")]
//    [HasPermission("USER_VIEW_ALL")]
//    public async Task<IActionResult> GetUser(long userId)
//    {
//        return Ok(await _users.GetUserDetailsAsync(userId));
//    }

//    // --------------------------------------------------
//    // SELF PROFILE
//    // --------------------------------------------------
//    [HttpGet("me")]
//    [HasPermission("USER_VIEW_SELF")]
//    public async Task<IActionResult> GetMyProfile()
//    {
//        var userId = User.GetUserId();
//        return Ok(await _users.GetUserDetailsAsync(userId));
//    }

//    // --------------------------------------------------
//    // MY TEAM (MANAGER)
//    // --------------------------------------------------
//    [HttpGet("me/team")]
//    [HasPermission("USER_VIEW_TEAM")]
//    public async Task<IActionResult> GetMyTeam()
//    {
//        var managerId = User.GetUserId();
//        return Ok(await _users.GetTeamByManagerAsync(managerId));
//    }

//    // --------------------------------------------------
//    // LOCK USER (ADMIN)
//    // --------------------------------------------------
//    [HttpPut("{userId:long}/lock")]
//    [HasPermission("USER_LOCK")]
//    public async Task<IActionResult> LockUser(long userId, LockUserDto dto)
//    {
//        var actorId = User.GetUserId();
//        await _users.LockUserAsync(userId, dto.Reason, actorId);
//        return Ok();
//    }

//    // --------------------------------------------------
//    // UNLOCK USER (ADMIN)
//    // --------------------------------------------------
//    [HttpPut("{userId:long}/unlock")]
//    [HasPermission("USER_UNLOCK")]
//    public async Task<IActionResult> UnlockUser(long userId)
//    {
//        var actorId = User.GetUserId();
//        await _users.UnlockUserAsync(userId, actorId);
//        return Ok();
//    }
//}


using CRM_Backend.DTOs.Users;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Security.Extensions;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CRM_Backend.Controllers;

[ApiController]
[Route("api/users")]

[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _users;
    private readonly IAdminUserListService _listService;
    private readonly IAdminUserDetailsService _detailsService;
    private readonly IAdminUserSecurityService _securityService;
    private readonly IAdminUserAuditLogService _auditLogService;

    public UsersController(
        IUserManagementService users,
        IAdminUserListService listService,
        IAdminUserDetailsService detailsService,
        IAdminUserSecurityService securityService,
        IAdminUserAuditLogService auditLogService)
    {
        _users = users;
        _listService = listService;
        _detailsService = detailsService;
        _securityService = securityService;
        _auditLogService = auditLogService;
    }

    // --------------------------------------------------
    // SELF PROFILE
    // --------------------------------------------------
    [HttpGet("me")]
    [HasPermission("USER_VIEW_SELF")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.GetUserId();
        return Ok(await _detailsService.GetUserDetailsAsync(userId));
    }

    // --------------------------------------------------
    // MY TEAM (MANAGER)
    // --------------------------------------------------
    [HttpGet("me/team")]
    [HasPermission("USER_VIEW_TEAM")]
    public async Task<IActionResult> GetMyTeam()
    {
        var managerId = User.GetUserId();
        return Ok(await _users.GetTeamByManagerAsync(managerId));
    }

    // --------------------------------------------------
    // GLOBAL USER LIST (ADMIN)
    // --------------------------------------------------
    [HttpGet]
    [HasPermission("USER_VIEW_ALL")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        return Ok(await _listService.GetUsersAsync(page, pageSize));
    }

    // --------------------------------------------------
    // USER DETAILS (ADMIN)
    // --------------------------------------------------
    [HttpGet("{userId:long}")]
    [HasPermission("USER_VIEW_ALL")]
    public async Task<IActionResult> GetUser(long userId)
    {
        return Ok(await _detailsService.GetUserDetailsAsync(userId));
    }

    // --------------------------------------------------
    // USER SECURITY (ADMIN / SECURITY TEAM)
    // --------------------------------------------------
    [HttpGet("{userId:long}/security")]
    [HasPermission("SECURITY_VIEW")]
    public async Task<IActionResult> GetUserSecurity(long userId)
    {
        return Ok(await _securityService.GetUserSecurityAsync(userId));
    }

    // --------------------------------------------------
    // USER AUDIT LOGS (ADMIN / AUDIT TEAM)
    // --------------------------------------------------
    [HttpGet("{userId:long}/audit-logs")]
    [HasPermission("AUDIT_LOG_VIEW")]
    public async Task<IActionResult> GetUserAuditLogs(long userId)
    {
        return Ok(await _auditLogService.GetUserAuditLogsAsync(userId));
    }

    // --------------------------------------------------
    // CREATE USER (ADMIN)
    // --------------------------------------------------
    [HttpPost]
    [HasPermission("USER_CREATE")]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var actorId = User.GetUserId();
        var userId = await _users.CreateUserAsync(dto, actorId);
        return Ok(new { userId });
    }

    // --------------------------------------------------
    // UPDATE USER (ADMIN)
    // --------------------------------------------------
    [HttpPut("{userId:long}")]
    [HasPermission("USER_UPDATE")]
    public async Task<IActionResult> UpdateUser(long userId, UpdateUserDto dto)
    {
        var actorId = User.GetUserId();
        await _users.UpdateUserAsync(userId, dto, actorId);
        return Ok();
    }

    // --------------------------------------------------
    // ASSIGN MANAGER (ADMIN)
    // --------------------------------------------------
    [HttpPut("{userId:long}/manager")]
    [HasPermission("USER_ASSIGN_MANAGER")]
    public async Task<IActionResult> AssignManager(long userId, AssignManagerDto dto)
    {
        await _users.AssignManagerAsync(userId, dto.ManagerId);
        return Ok();
    }

    // --------------------------------------------------
    // LOCK USER (ADMIN)
    // --------------------------------------------------
    [HttpPut("{userId:long}/lock")]
    [HasPermission("USER_LOCK")]
    public async Task<IActionResult> LockUser(long userId, LockUserDto dto)
    {
        var actorId = User.GetUserId();
        await _users.LockUserAsync(userId, dto.Reason, actorId);
        return Ok();
    }

    // --------------------------------------------------
    // UNLOCK USER (ADMIN)
    // --------------------------------------------------
    [HttpPut("{userId:long}/unlock")]
    [HasPermission("USER_UNLOCK")]
    public async Task<IActionResult> UnlockUser(long userId)
    {
        var actorId = User.GetUserId();
        await _users.UnlockUserAsync(userId, actorId);
        return Ok();
    }

    // --------------------------------------------------
    // MANAGERS DROPDOWN (ADMIN)
    // --------------------------------------------------
    [HttpGet("managers")]
    [HasPermission("USER_VIEW_ALL")]
    public async Task<IActionResult> GetManagers([FromQuery] string domainCode)
    {
        return Ok(await _users.GetManagersByDomainAsync(domainCode));
    }

    // --------------------------------------------------
    // DEBUG CLAIMS (TEMPORARY - REMOVE IN PROD)
    // --------------------------------------------------
    [HttpGet("debug/claims")]
    [Authorize]
    public IActionResult GetAuthenticatedUserClaims()
    {
        var claims = User.Claims
            .Select(c => new { c.Type, c.Value })
            .ToList();

        return Ok(new
        {
            Authenticated = User?.Identity?.IsAuthenticated ?? false,
            Claims = claims
        });
    }
}
