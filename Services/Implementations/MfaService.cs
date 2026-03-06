using CRM_Backend.DTOs.Auth;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace CRM_Backend.Services.Implementations;

public class MfaService : IMfaService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;
    private readonly IUserPasswordRepository _passwordRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<MfaService> _logger;

    public MfaService(
        IUserRepository userRepository,
        IUserSecurityRepository userSecurityRepository,
        IUserPasswordRepository passwordRepository,
        IPasswordService passwordService,
        ILogger<MfaService> logger)
    {
        _userRepository = userRepository;
        _userSecurityRepository = userSecurityRepository;
        _passwordRepository = passwordRepository;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<MfaResponseDto> EnableMfaAsync(long userId, string mfaType, string password)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Verify password
        var currentPassword = await _passwordRepository.GetCurrentPasswordAsync(userId);
        if (currentPassword == null || !_passwordService.VerifyPassword(password, currentPassword.PasswordHash))
            throw new UnauthorizedException("Invalid password");

        // Validate MFA type
        var validMfaTypes = new[] { "EMAIL", "SMS", "AUTHENTICATOR" };
        if (!validMfaTypes.Contains(mfaType.ToUpper()))
            throw new ValidationException("Invalid MFA type. Allowed: EMAIL, SMS, AUTHENTICATOR");

        // Get user security
        var security = await _userSecurityRepository.GetByUserIdAsync(userId);
        if (security == null)
            throw new NotFoundException("User security record not found");

        // Check if already enabled
        if (security.MfaEnabled)
            throw new ValidationException("MFA is already enabled for this user");

        // Enable MFA
        security.MfaEnabled = true;
        security.MfaType = mfaType.ToUpper();
        await _userSecurityRepository.UpdateAsync(security);

        _logger.LogInformation($"MFA enabled for user {userId} with type {mfaType}");

        // Generate recovery codes
        var recoveryCodes = GenerateRecoveryCodes(10);

        return new MfaResponseDto
        {
            MfaEnabled = true,
            MfaType = mfaType,
            RecoveryCodes = recoveryCodes,
            Message = $"MFA has been enabled successfully using {mfaType}. Save your recovery codes in a safe place."
        };
    }

    public async Task DisableMfaAsync(long userId, string password)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Verify password
        var currentPassword = await _passwordRepository.GetCurrentPasswordAsync(userId);
        if (currentPassword == null || !_passwordService.VerifyPassword(password, currentPassword.PasswordHash))
            throw new UnauthorizedException("Invalid password");

        // Get user security
        var security = await _userSecurityRepository.GetByUserIdAsync(userId);
        if (security == null)
            throw new NotFoundException("User security record not found");

        // Check if MFA is enabled
        if (!security.MfaEnabled)
            throw new ValidationException("MFA is not currently enabled for this user");

        // Disable MFA
        security.MfaEnabled = false;
        security.MfaType = null;
        await _userSecurityRepository.UpdateAsync(security);

        _logger.LogInformation($"MFA disabled for user {userId}");
    }

    public async Task<(bool Enabled, string? Type)> GetMfaStatusAsync(long userId)
    {
        var security = await _userSecurityRepository.GetByUserIdAsync(userId);
        if (security == null)
            throw new NotFoundException("User security record not found");

        return (security.MfaEnabled, security.MfaType);
    }

    public List<string> GenerateRecoveryCodes(int count = 10)
    {
        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            // Generate 8-character recovery codes
            var bytes = RandomNumberGenerator.GetBytes(6);
            var code = Convert.ToBase64String(bytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 8)
                .ToUpper();

            codes.Add(code);
        }
        return codes;
    }

    public async Task<bool> VerifyMfaCodeAsync(long userId, string code)
    {
        // This is a placeholder for actual MFA verification
        // Implementation depends on MFA type (EMAIL, SMS, AUTHENTICATOR)
        // For now, return true to allow flexibility

        var security = await _userSecurityRepository.GetByUserIdAsync(userId);
        if (security == null || !security.MfaEnabled)
            return false;

        // TODO: Implement actual MFA verification based on type
        // - EMAIL: Verify code sent to email
        // - SMS: Verify code sent to SMS
        // - AUTHENTICATOR: Verify TOTP code

        _logger.LogInformation($"MFA code verification for user {userId} - type: {security.MfaType}");
        return true;
    }
}
