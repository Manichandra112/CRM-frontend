namespace CRM_Backend.DTOs.Roles;

public class CreateRoleDto
{
    public string RoleName { get; set; } = null!;
    public string RoleCode { get; set; } = null!;
    public string DomainCode { get; set; } = default!;
    public string ModuleCode { get; set; } = null!;
    public string? Description { get; set; }
}
