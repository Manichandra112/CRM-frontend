using Microsoft.AspNetCore.Authorization;

namespace CRM_Backend.Security.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
        {
            Policy = "PERMISSION";
            Permission = permission;
        }

        public string Permission { get; }
    }

}
