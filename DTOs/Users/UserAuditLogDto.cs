namespace CRM_Backend.DTOs.Users
{
    public class UserAuditLogDto
    {
        public string Action { get; set; } = null!;
        public string Module { get; set; } = null!;
        public string? Metadata { get; set; }

        public string? ActorName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
