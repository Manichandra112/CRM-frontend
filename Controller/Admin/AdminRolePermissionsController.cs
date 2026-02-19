//using CRM_Backend.DTOs.Roles;
//using CRM_Backend.Security.Authorization;
//using CRM_Backend.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using CRM_Backend.Security.Extensions;

//namespace CRM_Backend.Controller.Admin;

//[ApiController]
//[Route("api/admin/role-permissions")]

//[Authorize(Policy = "ACCOUNT_ACTIVE")]
//[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

//public class AdminRolePermissionsController : ControllerBase
//{
//    private readonly IRolePermissionService _service;

//    public AdminRolePermissionsController(IRolePermissionService service)
//    {
//        _service = service;
//    }

//    // ASSIGN PERMISSION TO ROLE
//    [HttpPost]
//    [HasPermission("PERMISSION_ASSIGN")]
//    public async Task<IActionResult> Assign(AssignPermissionDto dto)
//    {
//        var adminId = User.GetUserId();

//        await _service.AssignPermissionAsync(
//            dto.RoleCode,
//            dto.PermissionCode,
//            adminId
//        );

//        return Ok();
//    }


//    // REMOVE PERMISSION FROM ROLE
//    [HttpDelete]
//    [HasPermission("PERMISSION_ASSIGN")]
//    public async Task<IActionResult> Remove(AssignPermissionDto dto)
//    {
//        await _service.RemovePermissionAsync(
//            dto.RoleCode,
//            dto.PermissionCode
//        );

//        return Ok();
//    }

//    // VIEW PERMISSIONS BY ROLE ID
//    [HttpGet("by-role/{roleId:long}")]
//    [HasPermission("PERMISSION_VIEW")]
//    public async Task<IActionResult> GetPermissionsByRole(long roleId)
//    {
//        var permissionIds = await _service.GetPermissionIdsByRoleAsync(roleId);
//        return Ok(permissionIds);
//    }

//    // VIEW PERMISSIONS BY ROLE CODE
//    [HttpGet("by-role/code/{roleCode}")]
//    [HasPermission("PERMISSION_VIEW")]
//    public async Task<IActionResult> GetByRoleCode(string roleCode)
//    {
//        var result = await _service.GetRolePermissionsAsync(roleCode);
//        return Ok(result);
//    }
//}



using CRM_Backend.DTOs.Roles;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM_Backend.Security.Extensions;

namespace CRM_Backend.Controller.Admin;

/// <summary>
/// Provides administrative operations for assigning and managing 
/// permissions associated with roles.
/// </summary>
/// <remarks>
/// Access Requirements:
/// - Authenticated user
/// - ACCOUNT_ACTIVE policy
/// - PASSWORD_RESET_COMPLETED policy
/// - Specific PERMISSION_* permission depending on the operation
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
    /// Permission Required: PERMISSION_ASSIGN
    ///
    /// Sample Request:
    ///
    ///     POST /api/admin/role-permissions
    ///     {
    ///         "roleCode": "ADMIN",
    ///         "permissionCode": "USER_CREATE"
    ///     }
    ///
    /// The currently authenticated admin user is recorded as the actor.
    /// </remarks>
    /// <param name="dto">Role and permission mapping payload.</param>
    /// <response code="200">Permission successfully assigned to role.</response>
    /// <response code="400">Validation error.</response>
    /// <response code="404">Role or permission not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks PERMISSION_ASSIGN permission.</response>
    [HttpPost]
    [HasPermission("PERMISSION_ASSIGN")]
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
    /// Permission Required: PERMISSION_ASSIGN
    ///
    /// Sample Request:
    ///
    ///     DELETE /api/admin/role-permissions
    ///     {
    ///         "roleCode": "ADMIN",
    ///         "permissionCode": "USER_CREATE"
    ///     }
    /// </remarks>
    /// <param name="dto">Role and permission mapping payload.</param>
    /// <response code="200">Permission successfully removed from role.</response>
    /// <response code="400">Validation error.</response>
    /// <response code="404">Role or permission not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks PERMISSION_ASSIGN permission.</response>
    [HttpDelete]
    [HasPermission("PERMISSION_ASSIGN")]
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
    /// <param name="roleId">Unique identifier of the role.</param>
    /// <response code="200">Returns list of permission IDs assigned to the role.</response>
    /// <response code="404">Role not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks PERMISSION_VIEW permission.</response>
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
    /// <param name="roleCode">Unique code of the role.</param>
    /// <response code="200">Returns permissions assigned to the role.</response>
    /// <response code="404">Role not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks PERMISSION_VIEW permission.</response>
    [HttpGet("by-role/code/{roleCode}")]
    [HasPermission("PERMISSION_VIEW")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByRoleCode(string roleCode)
    {
        var result = await _service.GetRolePermissionsAsync(roleCode);
        return Ok(result);
    }
}
