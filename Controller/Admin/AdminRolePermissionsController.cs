using CRM_Backend.DTOs.Roles;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/role-permissions")]

[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

public class AdminRolePermissionsController : ControllerBase
{
    private readonly IRolePermissionService _service;

    public AdminRolePermissionsController(IRolePermissionService service)
    {
        _service = service;
    }

    // ASSIGN PERMISSION TO ROLE
    [HttpPost]
    [HasPermission("PERMISSION_ASSIGN")]
    public async Task<IActionResult> Assign(AssignPermissionDto dto)
    {
        var adminId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _service.AssignPermissionAsync(
            dto.RoleCode,
            dto.PermissionCode,
            adminId
        );

        return Ok();
    }

    // REMOVE PERMISSION FROM ROLE
    [HttpDelete]
    [HasPermission("PERMISSION_ASSIGN")]
    public async Task<IActionResult> Remove(AssignPermissionDto dto)
    {
        await _service.RemovePermissionAsync(
            dto.RoleCode,
            dto.PermissionCode
        );

        return Ok();
    }

    // VIEW PERMISSIONS BY ROLE ID
    [HttpGet("by-role/{roleId:long}")]
    [HasPermission("PERMISSION_VIEW")]
    public async Task<IActionResult> GetPermissionsByRole(long roleId)
    {
        var permissionIds = await _service.GetPermissionIdsByRoleAsync(roleId);
        return Ok(permissionIds);
    }

    // VIEW PERMISSIONS BY ROLE CODE
    [HttpGet("by-role/code/{roleCode}")]
    [HasPermission("PERMISSION_VIEW")]
    public async Task<IActionResult> GetByRoleCode(string roleCode)
    {
        var result = await _service.GetRolePermissionsAsync(roleCode);
        return Ok(result);
    }
}
