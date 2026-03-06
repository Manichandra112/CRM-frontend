namespace CRM_Backend.DTOs.Auth;

/// <summary>
/// Request to enable or disable MFA
/// </summary>
public class MfaRequestDto
{
    /// <summary>
    /// MFA type: "EMAIL", "SMS", "AUTHENTICATOR"
    /// </summary>
    public string MfaType { get; set; } = null!;

    /// <summary>
    /// Current password for verification
    /// </summary>
    public string Password { get; set; } = null!;
}

/// <summary>
/// Response after enabling MFA
/// </summary>
public class MfaResponseDto
{
    /// <summary>
    /// Whether MFA is now enabled
    /// </summary>
    public bool MfaEnabled { get; set; }

    /// <summary>
    /// MFA type that was set
    /// </summary>
    public string MfaType { get; set; } = null!;

    /// <summary>
    /// Recovery codes (backup codes) - shown only on first enable
    /// </summary>
    public List<string>? RecoveryCodes { get; set; }

    /// <summary>
    /// Message for user
    /// </summary>
    public string Message { get; set; } = null!;
}

/// <summary>
/// Request to disable MFA
/// </summary>
public class DisableMfaRequestDto
{
    /// <summary>
    /// Current password for verification
    /// </summary>
    public string Password { get; set; } = null!;
}
