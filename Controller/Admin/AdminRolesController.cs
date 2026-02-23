

using CRM_Backend.DTOs.Roles;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

/// <summary>
/// Provides administrative operations for managing roles.
/// Roles aggregate permissions and are assigned to users.
/// </summary>
/// <remarks>
/// Access Requirements:
/// - Authenticated user
/// - ACCOUNT_ACTIVE policy
/// - PASSWORD_RESET_COMPLETED policy
/// - Specific ROLE_* permission depending on the operation
/// </remarks>
[ApiController]
[Route("api/admin/roles")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
[Produces("application/json")]
public class AdminRolesController : ControllerBase
{
    private readonly IRoleService _roles;

    public AdminRolesController(IRoleService roles)
    {
        _roles = roles;
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <remarks>
    /// Permission Required: ROLE_CREATE
    ///
    /// Sample Request:
    ///
    ///     POST /api/admin/roles
    ///     {
    ///         "roleCode": "ADMIN",
    ///         "roleName": "Administrator",
    ///         "domainCode": "SYSTEM"
    ///     }
    /// </remarks>
    /// <param name="dto">Role creation payload.</param>
    /// <response code="200">Role successfully created.</response>
    /// <response code="400">Validation error.</response>
    /// <response code="409">Role already exists.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks ROLE_CREATE permission.</response>
    [HttpPost]
    [HasPermission("ROLE_CREATE")]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        return Ok(await _roles.CreateAsync(dto));
    }

    /// <summary>
    /// Retrieves roles.
    /// </summary>
    /// <remarks>
    /// Permission Required: ROLE_VIEW
    ///
    /// Optional query parameter:
    /// - domainCode → Filters roles by domain.
    ///
    /// Examples:
    /// 
    ///     GET /api/admin/roles
    ///     GET /api/admin/roles?domainCode=SYSTEM
    /// </remarks>
    /// <param name="domainCode">Optional domain code filter.</param>
    /// <response code="200">Returns list of roles.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks ROLE_VIEW permission.</response>
    [HttpGet]
    [HasPermission("ROLE_VIEW")]
    [ProducesResponseType(typeof(IEnumerable<RoleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] string? domainCode)
    {
        if (!string.IsNullOrWhiteSpace(domainCode))
            return Ok(await _roles.GetByDomainAsync(domainCode));

        return Ok(await _roles.GetAllAsync());
    }

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <remarks>
    /// Permission Required: ROLE_UPDATE
    ///
    /// Only editable role properties can be modified.
    ///
    /// Sample Request:
    ///
    ///     PUT /api/admin/roles/5
    ///     {
    ///         "roleName": "Updated Role Name",
    ///         "isActive": true
    ///     }
    /// </remarks>
    /// <param name="id">Unique identifier of the role.</param>
    /// <param name="dto">Role update payload.</param>
    /// <response code="204">Role successfully updated.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="404">Role not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks ROLE_UPDATE permission.</response>
    [HttpPut("{id:long}")]
    [HasPermission("ROLE_UPDATE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateRoleDto dto)
    {
        await _roles.UpdateAsync(id, dto);
        return NoContent();
    }
}
