using System.ComponentModel.DataAnnotations;

namespace CRM_Backend.DTOs.Users;

public class UpdateUserDto
{
   

    [StringLength(100)]
    public string? Department { get; set; }

    [StringLength(100)]
    public string? Designation { get; set; }

    [StringLength(30)]
    public string? EmploymentType { get; set; }

    [StringLength(30)]
    public string? WorkShift { get; set; }

    [StringLength(100)]
    public string? AssignedRegion { get; set; }

    [StringLength(100)]
    public string? AssignedBranch { get; set; }

    [StringLength(255)]
    public string? Remarks { get; set; }

  
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [Phone]
    [StringLength(20)]
    public string? MobileNumber { get; set; }
}
