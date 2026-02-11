using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace CRM_Backend.Security.Extensions
{
    public static class ClaimsExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(sub))
                throw new UnauthorizedAccessException("User ID claim missing");

            return long.Parse(sub);
        }
    }
}
