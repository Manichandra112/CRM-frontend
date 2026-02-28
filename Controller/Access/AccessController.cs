using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Backend.Controller.Access
{
    /// <summary>
    /// Provides access information for the authenticated user.
    /// Used by frontend to build authorization-aware UI.
    /// </summary>
    /// <remarks>
    /// Requires authenticated user.
    /// Returns roles, permissions, and other access metadata
    /// required for client-side authorization logic.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/access")]
    [Authorize(Policy = "ACCOUNT_ACTIVE")]

    [Produces("application/json")]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _accessService;

        public AccessController(IAccessService accessService)
        {
            _accessService = accessService;
        }

        /// <summary>
        /// Retrieves access details for the authenticated user.
        /// </summary>
        /// <remarks>
        /// Extracts user ID from JWT token and returns:
        /// - Assigned roles
        /// - Effective permissions
        /// - Domain-level access
        /// - Additional authorization metadata
        ///
        /// Typically called immediately after login.
        /// </remarks>
        /// <response code="200">Returns access metadata for authenticated user.</response>
        /// <response code="401">User is not authenticated or token invalid.</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("User id not found in token");

            long userId = long.Parse(userIdClaim.Value);

            var response = await _accessService.GetMeAsync(userId);

            return Ok(response);
        }
    }
}