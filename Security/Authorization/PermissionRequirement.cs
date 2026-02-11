namespace CRM_Backend.Security.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string? Permission { get; }

        public PermissionRequirement(string? permission = null)
        {
            Permission = permission;
        }
    }


}
