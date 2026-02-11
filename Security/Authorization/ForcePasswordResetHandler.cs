namespace CRM_Backend.Security.Authorization
{
    using CRM_Backend.Repositories.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using System.IdentityModel.Tokens.Jwt;

    public class ForcePasswordResetHandler
        : AuthorizationHandler<ForcePasswordResetRequirement>
    {
        private readonly IUserSecurityRepository _security;

        public ForcePasswordResetHandler(IUserSecurityRepository security)
        {
            _security = security;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ForcePasswordResetRequirement requirement)
        {
            // ✅ JWT SUB claim (because inbound mapping is disabled)
            var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null)
                return;

            if (!long.TryParse(userIdClaim.Value, out var userId))
                return;

            var security = await _security.GetByUserIdAsync(userId);

            if (security == null)
                return;

            // Allow access ONLY when password reset is NOT required
            if (!security.ForcePasswordReset)
                context.Succeed(requirement);
        }
    }
}
