using CRM_Backend.DTOs.Roles;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

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
    ///         "roleCode": "HR_MANAGER",
    ///         "roleName": "HR Manager",
    ///         "domainCode": "SYSTEM",
    ///         "moduleCode": "HR",
    ///         "description": "Handles HR operations"
    ///     }
    /// </remarks>
    [HttpPost]
    [HasPermission("ROLE_CREATE")]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var result = await _roles.CreateAsync(dto);
        return Ok(result);
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
    /// Sample Request:
    ///
    ///     PUT /api/admin/roles/5
    ///     {
    ///         "isActive": true
    ///     }
    /// </remarks>
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