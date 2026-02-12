
using CRM_Backend.Domain.Entities;
using CRM_Backend.Exceptions;
using CRM_Backend.Security.Jwt;
using CRM_Backend.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRM_Backend.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    //public string GenerateAccessToken(
    // User user,
    // IEnumerable<string> roles,
    // IEnumerable<string> permissions)
    //{
    //    var now = DateTime.UtcNow;

    //    var forcePasswordReset =
    //        user.Security != null &&
    //        user.Security.ForcePasswordReset;

    //    var claims = new List<Claim>
    //{
    //    // 🔑 Identity
    //    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
    //    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),   // ✅ REQUIRED
    //    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    //    new Claim("username", user.Username),

    //    // 🔐 Password Reset UX Claims
    //    new Claim("pwd_reset_required", forcePasswordReset.ToString().ToLower()),
    //    new Claim("pwd_reset_completed", (!forcePasswordReset).ToString().ToLower()),

    //    // 🔒 Account Status
    //    new Claim("account_status", user.AccountStatus),

    //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //    new Claim(
    //        JwtRegisteredClaimNames.Iat,
    //        new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
    //        ClaimValueTypes.Integer64)
    //};

    //    foreach (var role in roles)
    //        claims.Add(new Claim(ClaimTypes.Role, role));

    //    foreach (var permission in permissions)
    //        claims.Add(new Claim("perm", permission));

    //    var key = new SymmetricSecurityKey(
    //        Encoding.UTF8.GetBytes(_settings.Key));

    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        _settings.Issuer,
    //        _settings.Audience,
    //        claims,
    //        now,
    //        now.AddMinutes(_settings.AccessTokenMinutes),
    //        creds
    //    );

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}


    public string GenerateAccessToken(
    User user,
    IEnumerable<string> roles,
    IEnumerable<string> permissions)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (user.AccountStatus != "ACTIVE")
            throw new ForbiddenException("User account is not active.");

        if (string.IsNullOrWhiteSpace(_settings.Key))
            throw new InvalidOperationException("JWT signing key is not configured.");

        var keyBytes = Encoding.UTF8.GetBytes(_settings.Key);

        if (keyBytes.Length < 32)
            throw new InvalidOperationException(
                "JWT signing key must be at least 32 characters long.");

        var now = DateTime.UtcNow;

        var forcePasswordReset =
            user.Security != null &&
            user.Security.ForcePasswordReset;

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("username", user.Username),

        new Claim("pwd_reset_required", forcePasswordReset ? "true" : "false"),
        new Claim("pwd_reset_completed", forcePasswordReset ? "false" : "true"),

        new Claim("account_status", user.AccountStatus),

        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(
            JwtRegisteredClaimNames.Iat,
            new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64)
    };

        foreach (var role in roles ?? Enumerable.Empty<string>())
            claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var permission in permissions ?? Enumerable.Empty<string>())
            claims.Add(new Claim("perm", permission));

        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _settings.Issuer,
            _settings.Audience,
            claims,
            now,
            now.AddMinutes(_settings.AccessTokenMinutes),
            creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}

