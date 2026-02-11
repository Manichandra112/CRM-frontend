

using CRM_Backend.DTOs.Users;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    // ASSIGN ROLE TO USER
    [HttpPost]
    [HasPermission("USER_ASSIGN_MANAGER")]
    public async Task<IActionResult> AssignRole(AssignUserRoleDto dto)
    {
        var adminId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _users.AssignRoleToUserAsync(dto.UserId, dto.RoleCode, adminId);
        return Ok();
    }

    // REMOVE ROLE FROM USER
    [HttpDelete]
    [HasPermission("USER_ASSIGN_MANAGER")]
    public async Task<IActionResult> RemoveRole(AssignUserRoleDto dto)
    {
        await _users.RemoveRoleFromUserAsync(dto.UserId, dto.RoleCode);
        return Ok();
    }

    // VIEW USER ROLES
    [HttpGet("{userId:long}")]
    [HasPermission("USER_VIEW")]
    public async Task<IActionResult> GetUserRoles(long userId)
    {
        var roles = await _users.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}
