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
            return Unauthorized("Missing refresh token");

        var tokenHash = RefreshTokenGenerator.HashToken(refreshToken);
        var stored = await _refreshRepo.GetByHashAsync(tokenHash);

        if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        var currentUserAgent = Request.Headers.UserAgent.ToString();
        var fingerprint = _fingerprintService.Generate(currentUserAgent);

        if (stored.DeviceFingerprint != fingerprint)
            return Unauthorized("Invalid refresh token");

        var user = await _userRepo.GetByIdAsync(stored.UserId);
        if (user == null)
            return Unauthorized("User not found");

        var revoked = await _refreshRepo.RevokeIfActiveAsync(stored.RefreshTokenId);
        if (!revoked)
            return Unauthorized("Invalid refresh token");

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

        Response.Cookies.Append(
            "refreshToken",
            rawRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
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
