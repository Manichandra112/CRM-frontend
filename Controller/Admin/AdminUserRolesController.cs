using CRM_Backend.DTOs.Users;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Security.Extensions;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/user-roles")]

[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

public class AdminUserRolesController : ControllerBase
{
    private readonly IUserManagementService _users;

    public AdminUserRolesController(IUserManagementService users)
    {
        _users = users;
    }

    // --------------------------------------------------
    // ASSIGN ROLE TO USER (ADMIN)
    // --------------------------------------------------
    [HttpPost]
    [HasPermission("USER_ASSIGN_ROLE")]
    public async Task<IActionResult> AssignRole(AssignUserRoleDto dto)
    {
        var adminId = User.GetUserId();

        await _users.AssignRoleToUserAsync(dto.UserId, dto.RoleCode, adminId);
        return Ok();
    }

    // --------------------------------------------------
    // REMOVE ROLE FROM USER (ADMIN)
    // --------------------------------------------------
    [HttpDelete]
    [HasPermission("USER_ASSIGN_ROLE")]
    public async Task<IActionResult> RemoveRole(AssignUserRoleDto dto)
    {
        await _users.RemoveRoleFromUserAsync(dto.UserId, dto.RoleCode);
        return Ok();
    }

    // --------------------------------------------------
    // VIEW USER ROLES (ADMIN)
    // --------------------------------------------------
    [HttpGet("{userId:long}")]
    [HasPermission("USER_VIEW_ALL")]
    public async Task<IActionResult> GetUserRoles(long userId)
    {
        var roles = await _users.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}
