using System.ComponentModel.DataAnnotations;

namespace CRM_Backend.DTOs.Users;

public class UserProfileDto
{
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(20)]
    public string? MobileNumber { get; set; }
}
