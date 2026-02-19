namespace CRM_Backend.DTOs.Self
{
    public class SelfSecurityOverviewDto
    {
        public bool MfaEnabled { get; set; }
        public string? MfaType { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }
        public string? LastLoginDevice { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? PasswordLastChangedAt { get; set; }
    }

}
