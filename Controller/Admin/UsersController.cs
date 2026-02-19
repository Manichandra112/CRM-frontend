





using CRM_Backend.DTOs.Users;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Security.Extensions;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CRM_Backend.Controllers;

/// <summary>
/// Provides user operations including self-service actions,
/// managerial views, and administrative user management.
/// </summary>
/// <remarks>
/// Access Requirements:
/// - Authenticated user
/// - ACCOUNT_ACTIVE policy
/// - PASSWORD_RESET_COMPLETED policy
/// - Specific USER_* / SECURITY_* / AUDIT_* permission depending on endpoint
/// </remarks>
[ApiController]
[Route("api/users")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
[Produces("application/json")]
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

    /// <summary>
    /// Retrieves the authenticated user's profile.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_VIEW_SELF
    /// </remarks>
    //[HttpGet("me")]
    //[HasPermission("USER_VIEW_SELF")]
    //[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetMyProfile()
    //{
    //    var userId = User.GetUserId();
    //    return Ok(await _detailsService.GetUserDetailsAsync(userId));
    //}

    /// <summary>
    /// Retrieves the team members reporting to the authenticated manager.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_VIEW_TEAM
    /// </remarks>
    //[HttpGet("me/team")]
    //[HasPermission("USER_VIEW_TEAM")]
    //[ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetMyTeam()
    //{
    //    var managerId = User.GetUserId();
    //    return Ok(await _users.GetTeamByManagerAsync(managerId));
    //}

    /// <summary>
    /// Updates the authenticated user's profile.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_UPDATE_SELF
    /// </remarks>
    //[HttpPut("me")]
    //[HasPermission("USER_UPDATE_SELF")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateSelfProfileDto dto)
    //{
    //    var userId = User.GetUserId();
    //    await _users.UpdateSelfProfileAsync(userId, dto);
    //    return Ok();
    //}

    // --------------------------------------------------
    // ADMIN USER OPERATIONS
    // --------------------------------------------------

    /// <summary>
    /// Retrieves paginated list of users (admin view).
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_VIEW_ALL
    /// Supports pagination and filtering.
    /// </remarks>
    [HttpGet]
    [HasPermission("USER_VIEW_ALL")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? role = null)
    {
        return Ok(await _listService.GetUsersAsync(page, pageSize, search, status, role));
    }

    /// <summary>
    /// Retrieves detailed information about a specific user.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_VIEW_ALL
    /// </remarks>
    [HttpGet("{userId:long}")]
    [HasPermission("USER_VIEW_ALL")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser(long userId)
    {
        return Ok(await _detailsService.GetUserDetailsAsync(userId));
    }

    /// <summary>
    /// Retrieves security metadata for a specific user.
    /// </summary>
    /// <remarks>
    /// Permission Required: SECURITY_VIEW
    /// </remarks>
    [HttpGet("{userId:long}/security")]
    [HasPermission("SECURITY_VIEW")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserSecurity(long userId)
    {
        return Ok(await _securityService.GetUserSecurityAsync(userId));
    }

    /// <summary>
    /// Retrieves audit logs related to a specific user.
    /// </summary>
    /// <remarks>
    /// Permission Required: AUDIT_LOG_VIEW
    /// </remarks>
    [HttpGet("{userId:long}/audit-logs")]
    [HasPermission("AUDIT_LOG_VIEW")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserAuditLogs(long userId)
    {
        return Ok(await _auditLogService.GetUserAuditLogsAsync(userId));
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_CREATE
    /// </remarks>
    [HttpPost]
    [HasPermission("USER_CREATE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var actorId = User.GetUserId();
        var userId = await _users.CreateUserAsync(dto, actorId);
        return Ok(new { userId });
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_UPDATE
    /// </remarks>
    [HttpPut("{userId:long}")]
    [HasPermission("USER_UPDATE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(long userId, [FromBody] UpdateUserDto dto)
    {
        var actorId = User.GetUserId();
        await _users.UpdateUserAsync(userId, dto, actorId);
        return Ok();
    }

    /// <summary>
    /// Assigns a manager to a user.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_ASSIGN_MANAGER
    /// </remarks>
    [HttpPut("{userId:long}/manager")]
    [HasPermission("USER_ASSIGN_MANAGER")]
    public async Task<IActionResult> AssignManager(long userId, [FromBody] AssignManagerDto dto)
    {
        var actorUserId = long.Parse(User.FindFirst("sub")!.Value);

        await _users.AssignManagerAsync(userId, dto.ManagerId, actorUserId);

        return Ok();
    }


    /// <summary>
    /// Locks a user account.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_LOCK
    /// </remarks>
    [HttpPut("{userId:long}/lock")]
    [HasPermission("USER_LOCK")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LockUser(long userId, [FromBody] LockUserDto dto)
    {
        var actorId = User.GetUserId();
        await _users.LockUserAsync(userId, dto.Reason, actorId);
        return Ok();
    }

    /// <summary>
    /// Unlocks a user account.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_UNLOCK
    /// </remarks>
    [HttpPut("{userId:long}/unlock")]
    [HasPermission("USER_UNLOCK")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UnlockUser(long userId)
    {
        var actorId = User.GetUserId();
        await _users.UnlockUserAsync(userId, actorId);
        return Ok();
    }

    /// <summary>
    /// Retrieves list of managers filtered by domain.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_VIEW_ALL
    /// </remarks>
    [HttpGet("managers")]
    [HasPermission("USER_VIEW_ALL")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManagers([FromQuery] string domainCode)
    {
        return Ok(await _users.GetManagersByDomainAsync(domainCode));
    }

    /// <summary>
    /// Returns authenticated user's JWT claims.
    /// </summary>
    /// <remarks>
    /// Debug endpoint — should be disabled in production.
    /// </remarks>
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
