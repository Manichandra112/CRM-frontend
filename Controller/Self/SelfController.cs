using CRM_Backend.DTOs.Auth;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Security.Extensions;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers.Self
{
    [ApiController]
    [Authorize]
    [Route("api/self")]
    [Authorize(Policy = "ACCOUNT_ACTIVE")]
    [Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
    [Produces("application/json")]
    public class SelfController : ControllerBase
    {
        private readonly IUserSelfService _self;

        public SelfController(IUserSelfService self)
        {
            _self = self;
        }

        // --------------------------------------------------
        // PROFILE
        // --------------------------------------------------

        /// <summary>
        /// Retrieves the authenticated user's profile.
        /// </summary>
        [HttpGet("profile")]
        [HasPermission("USER_VIEW_SELF")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.GetUserId();
            var result = await _self.GetProfileAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Updates the authenticated user's profile information.
        /// </summary>
        [HttpPatch("profile")]
        [HasPermission("USER_UPDATE_SELF")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateSelfProfileDto dto)
        {
            var userId = User.GetUserId();
            await _self.UpdateProfileAsync(userId, dto);
            return Ok();
        }

        // --------------------------------------------------
        // TEAM (if user is a manager)
        // --------------------------------------------------

        /// <summary>
        /// Retrieves team members reporting to the authenticated user.
        /// </summary>
        [HttpGet("team")]
        [HasPermission("USER_VIEW_TEAM")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTeam()
        {
            var userId = User.GetUserId();
            var team = await _self.GetTeamAsync(userId);
            return Ok(team);
        }

        // --------------------------------------------------
        // SECURITY OVERVIEW
        // --------------------------------------------------

        /// <summary>
        /// Retrieves security overview of the authenticated user.
        /// </summary>
        [HttpGet("security")]
        [HasPermission("USER_VIEW_SELF")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSecurityOverview()
        {
            var userId = User.GetUserId();
            var security = await _self.GetSecurityOverviewAsync(userId);
            return Ok(security);
        }


    }
}
