namespace CRM_Backend.DTOs.Self
{
    public class SelfProfileDto
    {
        public IdentityInfo Identity { get; set; }
        public OrganizationInfo Organization { get; set; }
        public PersonalInfo Personal { get; set; }
        public PreferenceInfo Preferences { get; set; }
        public ManagerInfo? Manager { get; set; }
        public AccountInfo Account { get; set; }
    }

    public class IdentityInfo
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? EmployeeId { get; set; }
    }

    public class OrganizationInfo
    {
        public string Domain { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? EmploymentType { get; set; }
        public string? WorkShift { get; set; }
    }

    public class PersonalInfo
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? MobileNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }

    public class PreferenceInfo
    {
        public string? Language { get; set; }
        public string? Timezone { get; set; }
    }

    public class ManagerInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class AccountInfo
    {
        public string AccountStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AccessStartDate { get; set; }
        public DateTime? AccessEndDate { get; set; }
    }

}
