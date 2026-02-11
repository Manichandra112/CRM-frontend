namespace CRM_Backend.Security.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class PermissionHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // 🔥 Super admin override
            if (context.User.HasClaim("perm", "CRM_FULL_ACCESS"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // 🔑 Permission required
            if (requirement.Permission != null &&
                context.User.HasClaim("perm", requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }



}
