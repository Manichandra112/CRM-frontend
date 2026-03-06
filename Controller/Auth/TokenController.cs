using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Security.Jwt;
using CRM_Backend.Security.Tokens;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CRM_Backend.Controller.Auth;

[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepo;
    private readonly IUserRoleRepository _userRoleRepo;
    private readonly ILogger<TokenController> _logger;
    private readonly IDeviceFingerprintService _fingerprintService;

    private readonly JwtSettings _jwtSettings;

    public TokenController(
        IRefreshTokenRepository refreshRepo,
        IJwtService jwtService,
        IUserRepository userRepo,
        IUserRoleRepository userRoleRepo,
        ILogger<TokenController> logger,
            IDeviceFingerprintService fingerprintService,
            IOptions<JwtSettings> jwtOptions)


    {
        _refreshRepo = refreshRepo;
        _jwtService = jwtService;
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _logger = logger;
        _fingerprintService = fingerprintService;
        _jwtSettings = jwtOptions.Value;

    }

    // --------------------------------------------------
    // REFRESH TOKEN
    // --------------------------------------------------
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning("Refresh token missing from cookies");
            return Unauthorized(new { error = "Missing refresh token" });
        }

        var tokenHash = RefreshTokenGenerator.HashToken(refreshToken);
        var stored = await _refreshRepo.GetByHashAsync(tokenHash);

        if (stored == null)
        {
            _logger.LogWarning("Refresh token not found or revoked in database");
            return Unauthorized(new { error = "Invalid or revoked refresh token" });
        }

        if (stored.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token expired");
            return Unauthorized(new { error = "Refresh token expired" });
        }

        var currentUserAgent = Request.Headers.UserAgent.ToString();
        var fingerprint = _fingerprintService.Generate(currentUserAgent);

        if (stored.DeviceFingerprint != fingerprint)
        {
            _logger.LogWarning("Device fingerprint mismatch - possible token hijack attempt");
            return Unauthorized(new { error = "Device verification failed" });
        }

        var user = await _userRepo.GetByIdAsync(stored.UserId);
        if (user == null)
        {
            _logger.LogWarning("User not found for refresh token");
            return Unauthorized(new { error = "User not found" });
        }

        var revoked = await _refreshRepo.RevokeIfActiveAsync(stored.RefreshTokenId);
        if (!revoked)
        {
            _logger.LogWarning("Failed to revoke old refresh token");
            return Unauthorized(new { error = "Token refresh failed" });
        }

        var roles = await _userRoleRepo.GetRoleCodesByUserIdAsync(user.UserId);
        var permissions = await _userRoleRepo.GetPermissionCodesByUserIdAsync(user.UserId);

        var newAccessToken = _jwtService.GenerateAccessToken(user, roles, permissions);

        var rawRefreshToken = RefreshTokenGenerator.GenerateToken();
        var hashedRefreshToken = RefreshTokenGenerator.HashToken(rawRefreshToken);

        await _refreshRepo.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            CreatedAt = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = currentUserAgent,
            DeviceFingerprint = fingerprint
        });

        _logger.LogInformation($"Token refreshed successfully for user {user.UserId}");

        Response.Cookies.Append(
            "refreshToken",
            rawRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !HttpContext.Request.IsHttps ? false : true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
            });

        return Ok(new
        {
            accessToken = newAccessToken
        });
    }


    // --------------------------------------------------
    // DEVICE FINGERPRINT (Stable + Secure)
    // --------------------------------------------------

}
