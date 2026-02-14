namespace CRM_Backend.DTOs.Users
{
    public class UpdateSelfProfileDto
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
        public string? LanguagePreference { get; set; }
        public string? Timezone { get; set; }
    }
}
