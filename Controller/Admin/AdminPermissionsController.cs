


//using CRM_Backend.DTOs.Permissions;
//using CRM_Backend.Security.Authorization;
//using CRM_Backend.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CRM_Backend.Controller.Admin;

//[ApiController]
//[Route("api/admin/permissions")]

//[Authorize(Policy = "ACCOUNT_ACTIVE")]
//[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

//public class AdminPermissionsController : ControllerBase
//{
//    private readonly IPermissionService _permissions;

//    public AdminPermissionsController(IPermissionService permissions)
//    {
//        _permissions = permissions;
//    }

//    // CREATE NEW PERMISSION
//    [HttpPost]
//    [HasPermission("PERMISSION_MANAGE")]
//    public async Task<IActionResult> Create(CreatePermissionDto dto)
//    {
//        var id = await _permissions.CreateAsync(dto);
//        return Ok(new { permissionId = id });
//    }

//    // VIEW ALL PERMISSIONS
//    [HttpGet]
//    [HasPermission("PERMISSION_VIEW")]
//    public async Task<IActionResult> GetAll()
//    {
//        return Ok(await _permissions.GetAllAsync());
//    }

//    // UPDATE PERMISSION
//    [HttpPut("{id:long}")]
//    [HasPermission("PERMISSION_MANAGE")]
//    public async Task<IActionResult> Update(long id, UpdatePermissionDto dto)
//    {
//        await _permissions.UpdateAsync(id, dto);
//        return NoContent();
//    }
//}


using CRM_Backend.DTOs.Permissions;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

/// <summary>
/// Provides administrative operations for managing system permissions.
/// Permissions define fine-grained access control rules within domains.
/// </summary>
/// <remarks>
/// Access Requirements:
/// - Authenticated user
/// - ACCOUNT_ACTIVE policy
/// - PASSWORD_RESET_COMPLETED policy
/// - Specific PERMISSION_* permission depending on the operation
/// </remarks>
[ApiController]
[Route("api/admin/permissions")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
[Produces("application/json")]
public class AdminPermissionsController : ControllerBase
{
    private readonly IPermissionService _permissions;

    public AdminPermissionsController(IPermissionService permissions)
    {
        _permissions = permissions;
    }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <remarks>
    /// Permission Required: PERMISSION_MANAGE
    ///
    /// Sample Request:
    ///
    ///     POST /api/admin/permissions
    ///     {
    ///         "permissionCode": "USER_CREATE",
    ///         "description": "Allows creating users",
    ///         "domainId": 1
    ///     }
    ///
    /// Returns the generated permission identifier.
    /// </remarks>
    /// <param name="dto">Permission creation payload.</param>
    /// <response code="200">Permission created successfully.</response>
    /// <response code="400">Validation error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks PERMISSION_MANAGE permission.</response>
    [HttpPost]
    [HasPermission("PERMISSION_MANAGE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto)
    {
        var id = await _permissions.CreateAsync(dto);
        return Ok(new { permissionId = id });
    }

    /// <summary>
    /// Retrieves all permissions.
    /// </summary>
    /// <remarks>
    /// Permission Required: PERMISSION_VIEW
    /// </remarks>
    /// <response code="200">Returns list of permissions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks PERMISSION_VIEW permission.</response>
    [HttpGet]
    [HasPermission("PERMISSION_VIEW")]
    [ProducesResponseType(typeof(IEnumerable<PermissionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _permissions.GetAllAsync());
    }

    /// <summary>
    /// Updates an existing permission.
    /// </summary>
    /// <remarks>
    /// Permission Required: PERMISSION_MANAGE
    ///
    /// Only editable fields can be modified.
    ///
    /// Sample Request:
    ///
    ///     PUT /api/admin/permissions/10
    ///     {
    ///       
    ///         "isActive": true
    ///     }
    /// </remarks>
    /// <param name="id">Unique identifier of the permission.</param>
    /// <param name="dto">Permission update payload.</param>
    /// <response code="204">Permission successfully updated.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="404">Permission not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks PERMISSION_MANAGE permission.</response>
    [HttpPut("{id:long}")]
    [HasPermission("PERMISSION_MANAGE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePermissionDto dto)
    {
        await _permissions.UpdateAsync(id, dto);
        return NoContent();
    }
}
