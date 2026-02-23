using CRM_Backend.Domain.Enums;
namespace CRM_Backend.DTOs.Users
{
    public class UpdateUserStatusDto
    {
        public AccountStatus Status { get; set; }
    }
}
