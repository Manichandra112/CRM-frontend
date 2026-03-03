namespace CRM_Backend.DTOs.Roles;

public class RoleResponseDto
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string RoleCode { get; set; } = null!;
    public string? Description { get; set; }

    public long DomainId { get; set; }

    public long? ModuleId { get; set; }
    public string? ModuleCode { get; set; }
    public string? ModuleName { get; set; }

    public bool IsSystemRole { get; set; }
    public bool Active { get; set; }
}
