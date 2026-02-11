//using CRM_Backend.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CRM_Backend.Controllers.Admin;

//[ApiController]
//[Route("api/admin/users")]
//[Authorize(Policy = "USER_VIEW")]
//[Authorize(Policy = "ACCOUNT_ACTIVE")]
//[Authorize(Policy = "CRM_FULL_ACCESS")]
//public class AdminUsersController : ControllerBase
//{
//    private readonly IAdminUserListService _listService;
//    private readonly IAdminUserDetailsService _detailsService;
//    private readonly IAdminUserSecurityService _securityService;
//    private readonly IAdminUserAuditLogService _auditLogService;

//    public AdminUsersController(
//        IAdminUserListService listService,
//        IAdminUserDetailsService detailsService,
//        IAdminUserSecurityService securityService,
//        IAdminUserAuditLogService auditLogService)
//    {
//        _listService = listService;
//        _detailsService = detailsService;
//        _securityService = securityService;
//        _auditLogService = auditLogService;
//    }

//    // LIST
//    [HttpGet]
//    public async Task<IActionResult> GetUsers(
//        [FromQuery] int page = 1,
//        [FromQuery] int pageSize = 25)
//    {
//        return Ok(await _listService.GetUsersAsync(page, pageSize));
//    }

//    // DETAILS
//    [HttpGet("{userId:long}")]
//    public async Task<IActionResult> GetUserDetails(long userId)
//    {
//        return Ok(await _detailsService.GetUserDetailsAsync(userId));
//    }

//    // SECURITY
//    [HttpGet("{userId:long}/security")]
//    public async Task<IActionResult> GetUserSecurity(long userId)
//    {
//        return Ok(await _securityService.GetUserSecurityAsync(userId));
//    }

//    // AUDIT LOGS
//    [HttpGet("{userId:long}/audit-logs")]
//    public async Task<IActionResult> GetUserAuditLogs(long userId)
//    {
//        return Ok(await _auditLogService.GetUserAuditLogsAsync(userId));
//    }
//}

using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]

[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserListService _listService;
    private readonly IAdminUserDetailsService _detailsService;
    private readonly IAdminUserSecurityService _securityService;
    private readonly IAdminUserAuditLogService _auditLogService;

    public AdminUsersController(
        IAdminUserListService listService,
        IAdminUserDetailsService detailsService,
        IAdminUserSecurityService securityService,
        IAdminUserAuditLogService auditLogService)
    {
        _listService = listService;
        _detailsService = detailsService;
        _securityService = securityService;
        _auditLogService = auditLogService;
    }

    // LIST USERS
    [HttpGet]
    [HasPermission("USER_VIEW")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        return Ok(await _listService.GetUsersAsync(page, pageSize));
    }

    // USER DETAILS
    [HttpGet("{userId:long}")]
    [HasPermission("USER_VIEW")]
    public async Task<IActionResult> GetUserDetails(long userId)
    {
        return Ok(await _detailsService.GetUserDetailsAsync(userId));
    }

    // USER SECURITY
    [HttpGet("{userId:long}/security")]
    [HasPermission("SECURITY_VIEW")]
    public async Task<IActionResult> GetUserSecurity(long userId)
    {
        return Ok(await _securityService.GetUserSecurityAsync(userId));
    }

    // USER AUDIT LOGS
    [HttpGet("{userId:long}/audit-logs")]
    [HasPermission("AUDIT_LOG_VIEW")]
    public async Task<IActionResult> GetUserAuditLogs(long userId)
    {
        return Ok(await _auditLogService.GetUserAuditLogsAsync(userId));
    }
}

