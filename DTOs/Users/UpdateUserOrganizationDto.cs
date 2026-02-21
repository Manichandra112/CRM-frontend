namespace CRM_Backend.DTOs.Users
{
    public class UpdateUserOrganizationDto
    {
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? EmploymentType { get; set; }
        public string? WorkShift { get; set; }
        public string? AssignedRegion { get; set; }
        public string? AssignedBranch { get; set; }
        public string? Remarks { get; set; }
        public DateTimeOffset? AccessStartDate { get; set; }
        public DateTimeOffset? AccessEndDate { get; set; }
        
    }
}
