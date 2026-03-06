using CRM_Backend.DTOs.Auth;

namespace CRM_Backend.Services.Interfaces;

public interface IMfaService
{
    /// <summary>
    /// Enable MFA for user
    /// </summary>
    Task<MfaResponseDto> EnableMfaAsync(long userId, string mfaType, string password);

    /// <summary>
    /// Disable MFA for user
    /// </summary>
    Task DisableMfaAsync(long userId, string password);

    /// <summary>
    /// Get MFA status for user
    /// </summary>
    Task<(bool Enabled, string? Type)> GetMfaStatusAsync(long userId);

    /// <summary>
    /// Generate recovery codes
    /// </summary>
    List<string> GenerateRecoveryCodes(int count = 10);

    /// <summary>
    /// Verify MFA code (placeholder for actual implementation)
    /// </summary>
    Task<bool> VerifyMfaCodeAsync(long userId, string code);
}
