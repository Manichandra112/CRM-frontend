namespace CRM_Backend.DTOs.Domains;

//public class CreateDomainDto
//{
//    public string DomainCode { get; set; } = null!;
//    public string DomainName { get; set; } = null!;
//}


public class CreateDomainDto
{
    /// <summary>
    /// Unique code representing the domain.
    /// </summary>
    public string DomainCode { get; set; } = null!;

    /// <summary>
    /// Display name of the domain.
    /// </summary>
    public string DomainName { get; set; } = null!;
}
