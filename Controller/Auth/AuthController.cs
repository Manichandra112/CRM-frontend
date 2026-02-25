
using CRM_Backend.DTOs.Auth;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Backend.Controller.Auth;

/// <summary>
/// Handles authentication and password lifecycle operations.
/// Includes login, password change, password reset, and token validation.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // --------------------------------------------------
    // LOGIN
    // --------------------------------------------------

    /// <summary>
    /// Authenticates a user and returns JWT access and refresh tokens.
    /// </summary>
    /// <remarks>
    /// Sample Request:
    ///
    ///     POST /api/auth/login
    ///     {
    ///         "username": "admin",
    ///         "password": "Admin@123"
    ///     }
    ///
    /// Captures client IP address and user agent for auditing.
    ///
    /// Failed authentication attempts are handled by global exception middleware.
    /// </remarks>
    /// <response code="200">Login successful. Returns tokens and user info.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await _authService.LoginAsync(
            request,
            ipAddress,
            userAgent
        );
        Response.Cookies.Append(
    "refreshToken",
    result.RefreshToken,
    new CookieOptions
    {
        HttpOnly = true,
        Secure = true, // true in production (HTTPS)
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
    }
);

        return Ok(new
        {
            accessToken = result.AccessToken,
            expiresAt = result.ExpiresAt
        });
    }

    // --------------------------------------------------
    // CHANGE PASSWORD (AUTHENTICATED)
    // --------------------------------------------------

    /// <summary>
    /// Changes password for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires valid authentication.
    ///
    /// After successful password change, user must re-login.
    /// </remarks>
    /// <response code="200">Password successfully changed.</response>
    /// <response code="400">Validation error.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = long.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _authService.ChangePasswordAsync(
            userId,
            dto.NewPassword
        );

        return Ok(new
        {
            message = "Password set successfully. Please login again."
        });
    }

    // --------------------------------------------------
    // FORGOT PASSWORD
    // --------------------------------------------------

    /// <summary>
    /// Initiates password reset process.
    /// </summary>
    /// <remarks>
    /// Always returns generic success response to prevent user enumeration attacks.
    /// </remarks>
    /// <response code="200">Reset process initiated.</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        await _authService.ForgotPasswordAsync(dto.Email);

        return Ok(new
        {
            message = "If the email exists, a reset link has been sent."
        });
    }

    // --------------------------------------------------
    // VALIDATE RESET TOKEN
    // --------------------------------------------------

    /// <summary>
    /// Validates password reset token.
    /// </summary>
    /// <param name="token">Password reset token.</param>
    /// <response code="200">Token is valid.</response>
    /// <response code="400">Token invalid or missing.</response>
    [HttpGet("validate-reset-token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { valid = false });

        var isValid = await _authService.ValidateResetTokenAsync(token);

        return isValid
            ? Ok(new { valid = true })
            : BadRequest(new { valid = false });
    }

    // --------------------------------------------------
    // RESET FORGOT PASSWORD
    // --------------------------------------------------

    /// <summary>
    /// Resets password using a valid reset token.
    /// </summary>
    /// <remarks>
    /// Requires a valid reset token obtained from forgot-password process.
    /// </remarks>
    /// <response code="200">Password reset successful.</response>
    /// <response code="400">Invalid token or validation error.</response>
    [HttpPost("reset-forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetForgotPassword([FromBody] ResetForgotPasswordDto dto)
    {
        await _authService.ResetForgotPasswordAsync(
            dto.Token,
            dto.NewPassword
        );

        return Ok(new
        {
            message = "Password reset successful. Please login."
        });
    }
}
