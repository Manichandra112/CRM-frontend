using CRM_Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CRM_Backend.Controllers.Debug;

[ApiController]
[Route("debug")]
public class DebugClaimsController : ControllerBase
{
    // Authenticated only, no extra policies or permission checks
    [HttpGet("claims")]
    [Authorize]
    public IActionResult GetClaims()
    {
        var claims = User.Claims
            .Select(c => new { Type = c.Type, Value = c.Value })
            .ToList();

        return Ok(new
        {
            Authenticated = User?.Identity?.IsAuthenticated ?? false,
            Claims = claims
        });
    }
}