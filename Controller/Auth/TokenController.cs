using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Security.Tokens;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

    public TokenController(
        IRefreshTokenRepository refreshRepo,
        IJwtService jwtService,
        IUserRepository userRepo,
        IUserRoleRepository userRoleRepo)
    {
        _refreshRepo = refreshRepo;
        _jwtService = jwtService;
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
    }

    // --------------------------------------------------
    // REFRESH TOKEN
    // --------------------------------------------------
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized("Missing refresh token");

        var tokenHash = RefreshTokenGenerator.HashToken(refreshToken);
        var stored = await _refreshRepo.GetByHashAsync(tokenHash);

        if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        var fingerprint = GenerateDeviceFingerprint();

        if (stored.DeviceFingerprint != fingerprint)
            return Unauthorized("Device mismatch");

        // 🔁 Rotate old refresh token
        await _refreshRepo.RevokeAsync(stored.RefreshTokenId);

        var user = await _userRepo.GetByIdAsync(stored.UserId);
        if (user == null)
            return Unauthorized("User not found");

        var roles = await _userRoleRepo.GetRoleCodesByUserIdAsync(user.UserId);
        var permissions = await _userRoleRepo.GetPermissionCodesByUserIdAsync(user.UserId);

        var newAccessToken = _jwtService.GenerateAccessToken(
            user,
            roles,
            permissions
        );

        // ✅ Generate NEW refresh token
        var rawRefreshToken = RefreshTokenGenerator.GenerateToken();
        var hashedRefreshToken = RefreshTokenGenerator.HashToken(rawRefreshToken);

        await _refreshRepo.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString(),
            DeviceFingerprint = fingerprint
        });

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = rawRefreshToken
        });
    }

    // --------------------------------------------------
    // DEVICE FINGERPRINT (Stable + Secure)
    // --------------------------------------------------
    private string GenerateDeviceFingerprint()
    {
        var raw = $"{HttpContext.Connection.RemoteIpAddress}:{Request.Headers.UserAgent}";
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash);
    }
}
