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
        private readonly IMfaService _mfaService;

        public SelfController(
            IUserSelfService self,
            IMfaService mfaService)
        {
            _self = self;
            _mfaService = mfaService;
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
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
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

        // --------------------------------------------------
        // MULTI-FACTOR AUTHENTICATION
        // --------------------------------------------------

        /// <summary>
        /// Enable MFA for authenticated user (optional feature)
        /// </summary>
        /// <remarks>
        /// MFA types available: EMAIL, SMS, AUTHENTICATOR
        /// User must provide password for verification
        /// </remarks>
        /// <response code="200">MFA enabled successfully with recovery codes</response>
        /// <response code="400">Invalid MFA type or MFA already enabled</response>
        /// <response code="401">Invalid password</response>
        [HttpPost("mfa/enable")]
        [HasPermission("USER_UPDATE_SELF")]
        [ProducesResponseType(typeof(MfaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EnableMfa([FromBody] MfaRequestDto request)
        {
            var userId = User.GetUserId();
            var result = await _mfaService.EnableMfaAsync(userId, request.MfaType, request.Password);
            return Ok(result);
        }

        /// <summary>
        /// Disable MFA for authenticated user
        /// </summary>
        /// <remarks>
        /// User must provide password for verification
        /// This is permanent - user will need to re-enable MFA to use it again
        /// </remarks>
        /// <response code="200">MFA disabled successfully</response>
        /// <response code="400">MFA not currently enabled</response>
        /// <response code="401">Invalid password</response>
        [HttpPost("mfa/disable")]
        [HasPermission("USER_UPDATE_SELF")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DisableMfa([FromBody] DisableMfaRequestDto request)
        {
            var userId = User.GetUserId();
            await _mfaService.DisableMfaAsync(userId, request.Password);
            return Ok(new { message = "MFA has been disabled successfully" });
        }

        /// <summary>
        /// Get MFA status for authenticated user
        /// </summary>
        /// <response code="200">Returns MFA status and type</response>
        [HttpGet("mfa/status")]
        [HasPermission("USER_VIEW_SELF")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMfaStatus()
        {
            var userId = User.GetUserId();
            var (enabled, type) = await _mfaService.GetMfaStatusAsync(userId);
            return Ok(new
            {
                mfaEnabled = enabled,
                mfaType = type
            });
        }
    }
}
