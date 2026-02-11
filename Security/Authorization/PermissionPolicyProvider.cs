using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CRM_Backend.Security.Authorization
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("PERMISSION:", StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName.Substring("PERMISSION:".Length);

                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}
