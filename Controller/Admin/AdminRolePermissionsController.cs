using CRM_Backend.DTOs.Roles;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM_Backend.Security.Extensions;

namespace CRM_Backend.Controller.Admin;

/// <summary>
/// Provides administrative operations for managing 
/// permissions assigned to roles.
/// </summary>
/// <remarks>
/// Access Requirements:
/// - Authenticated user
/// - ACCOUNT_ACTIVE policy
/// - PASSWORD_RESET_COMPLETED policy
/// - ROLE_PERMISSION_MANAGE or PERMISSION_VIEW depending on operation
/// </remarks>
[ApiController]
[Route("api/admin/role-permissions")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
[Produces("application/json")]
public class AdminRolePermissionsController : ControllerBase
{
    private readonly IRolePermissionService _service;

    public AdminRolePermissionsController(IRolePermissionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Assigns a permission to a role.
    /// </summary>
    /// <remarks>
    /// Permission Required: ROLE_PERMISSION_MANAGE
    /// </remarks>
    [HttpPost]
    [HasPermission("ROLE_PERMISSION_MANAGE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Assign([FromBody] AssignPermissionDto dto)
    {
        var adminId = User.GetUserId();

        await _service.AssignPermissionAsync(
            dto.RoleCode,
            dto.PermissionCode,
            adminId
        );

        return Ok();
    }

    /// <summary>
    /// Removes a permission from a role.
    /// </summary>
    /// <remarks>
    /// Permission Required: ROLE_PERMISSION_MANAGE
    /// </remarks>
    [HttpDelete]
    [HasPermission("ROLE_PERMISSION_MANAGE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Remove([FromBody] AssignPermissionDto dto)
    {
        await _service.RemovePermissionAsync(
            dto.RoleCode,
            dto.PermissionCode
        );

        return Ok();
    }

    /// <summary>
    /// Retrieves permission identifiers assigned to a specific role (by role ID).
    /// </summary>
    /// <remarks>
    /// Permission Required: PERMISSION_VIEW
    /// </remarks>
    [HttpGet("by-role/{roleId:long}")]
    [HasPermission("PERMISSION_VIEW")]
    [ProducesResponseType(typeof(IEnumerable<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPermissionsByRole(long roleId)
    {
        var permissionIds = await _service.GetPermissionIdsByRoleAsync(roleId);
        return Ok(permissionIds);
    }

    /// <summary>
    /// Retrieves detailed permissions assigned to a specific role (by role code).
    /// </summary>
    /// <remarks>
    /// Permission Required: PERMISSION_VIEW
    /// </remarks>
    [HttpGet("by-role/code/{roleCode}")]
    [HasPermission("PERMISSION_VIEW")]
    [ProducesResponseType(typeof(RolePermissionsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByRoleCode(string roleCode)
    {
        var result = await _service.GetRolePermissionsAsync(roleCode);
        return Ok(result);
    }
}