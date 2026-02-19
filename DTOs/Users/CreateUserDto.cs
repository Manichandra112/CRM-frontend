//namespace CRM_Backend.DTOs.Users;

//public class CreateUserDto
//{
//    public string Username { get; set; } = null!;
//    public string Email { get; set; } = null!;
//    public string DomainCode { get; set; } = null!;   // HR / SALES / SOCIALMEDIA

//    public string TemporaryPassword { get; set; } = null!;

//    public List<string> RoleCodes { get; set; } = new();

//    public UserProfileDto Profile { get; set; } = new();
//}



using System.ComponentModel.DataAnnotations;

namespace CRM_Backend.DTOs.Users;

public class CreateUserDto
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string DomainCode { get; set; } = null!;

    [StringLength(50)]
    public string? EmployeeId { get; set; }

    [Required]
    [MinLength(8)]
    public string TemporaryPassword { get; set; } = null!;

    [Required]
    [MinLength(1)]
    public List<string> RoleCodes { get; set; } = new();

    public UserProfileDto? Profile { get; set; }
}



