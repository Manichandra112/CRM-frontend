namespace CRM_Backend.DTOs.Module
{
    public class ModuleListDto
    {
        public long ModuleId { get; set; }
        public string ModuleCode { get; set; } = null!;
        public string ModuleName { get; set; } = null!;
    }
}
