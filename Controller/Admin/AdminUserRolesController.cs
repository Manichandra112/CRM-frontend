using CRM_Backend.DTOs.Roles;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Security.Extensions;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

/// <summary>
/// Provides administrative operations for assigning and managing
/// roles associated with users.
/// </summary>
/// <remarks>
/// Access Requirements:
/// - Authenticated user
/// - ACCOUNT_ACTIVE policy
/// - PASSWORD_RESET_COMPLETED policy
/// - Specific USER_* permission depending on the operation
/// </remarks>
[ApiController]
[Route("api/admin/user-roles")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
[Produces("application/json")]
public class AdminUserRolesController : ControllerBase
{
    private readonly IUserManagementService _users;

    public AdminUserRolesController(IUserManagementService users)
    {
        _users = users;
    }

    // --------------------------------------------------
    // ASSIGN ROLE TO USER
    // --------------------------------------------------

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_ROLE_MANAGE
    ///
    /// Sample Request:
    ///
    ///     POST /api/admin/user-roles
    ///     {
    ///         "userId": 15,
    ///         "roleCode": "ADMIN"
    ///     }
    ///
    /// The currently authenticated administrator is recorded
    /// as the actor for auditing purposes.
    /// </remarks>
    /// <param name="dto">User-role assignment payload.</param>
    /// <response code="200">Role successfully assigned to user.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="404">User or role not found.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User lacks USER_ROLE_MANAGE permission.</response>
    [HttpPost]
    [HasPermission("USER_ROLE_MANAGE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignRole([FromBody] AssignUserRoleDto dto)
    {
        var adminId = User.GetUserId();

        await _users.AssignRoleToUserAsync(dto.UserId, dto.RoleCode, adminId);
        return Ok();
    }

    // --------------------------------------------------
    // REMOVE ROLE FROM USER
    // --------------------------------------------------

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_ROLE_MANAGE
    ///
    /// Sample Request:
    ///
    ///     DELETE /api/admin/user-roles
    ///     {
    ///         "userId": 15,
    ///         "roleCode": "ADMIN"
    ///     }
    /// </remarks>
    /// <param name="dto">User-role removal payload.</param>
    /// <response code="200">Role successfully removed from user.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="404">User or role not found.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User lacks USER_ROLE_MANAGE permission.</response>
    [HttpDelete]
    [HasPermission("USER_ROLE_MANAGE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRole([FromBody] AssignUserRoleDto dto)
    {
        await _users.RemoveRoleFromUserAsync(dto.UserId, dto.RoleCode);
        return Ok();
    }

    // --------------------------------------------------
    // VIEW USER ROLES
    // --------------------------------------------------

    /// <summary>
    /// Retrieves roles assigned to a specific user.
    /// </summary>
    /// <remarks>
    /// Permission Required: USER_VIEW_ALL
    /// </remarks>
    /// <param name="userId">Unique identifier of the user.</param>
    /// <response code="200">Returns list of roles assigned to the user.</response>
    /// <response code="404">User not found.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User lacks USER_VIEW_ALL permission.</response>
    [HttpGet("{userId:long}")]
    [HasPermission("USER_VIEW_ALL")]
    [ProducesResponseType(typeof(IEnumerable<RoleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserRoles(long userId)
    {
        var roles = await _users.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}