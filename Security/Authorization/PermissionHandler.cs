using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CRM_Backend.Security.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissions = context.User
                .FindAll("perm")
                .Select(c => c.Value)
                .ToList();

            // SUPER ADMIN
            if (permissions.Contains("CRM_FULL_ACCESS"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // NORMAL CHECK
            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
