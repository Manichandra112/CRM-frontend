namespace CRM_Backend.DTOs.Permissions;

public class PermissionResponseDto
{
    public long PermissionId { get; set; }
    public string PermissionCode { get; set; } = default!;
    public string? Description { get; set; }
    public string Module { get; set; } = default!;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
