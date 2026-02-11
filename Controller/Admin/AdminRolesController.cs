
using CRM_Backend.DTOs.Roles;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/roles")]

[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

public class AdminRolesController : ControllerBase
{
    private readonly IRoleService _roles;

    public AdminRolesController(IRoleService roles)
    {
        _roles = roles;
    }

    // CREATE ROLE
    [HttpPost]
    [HasPermission("ROLE_CREATE")]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        return Ok(await _roles.CreateAsync(dto));
    }

    // VIEW ROLES
    [HttpGet]
    [HasPermission("ROLE_VIEW")]
    public async Task<IActionResult> GetAll([FromQuery] string? domainCode)
    {
        if (!string.IsNullOrWhiteSpace(domainCode))
            return Ok(await _roles.GetByDomainAsync(domainCode));

        return Ok(await _roles.GetAllAsync());
    }

    // UPDATE ROLE
    [HttpPut("{id:long}")]
    [HasPermission("ROLE_UPDATE")]
    public async Task<IActionResult> Update(long id, UpdateRoleDto dto)
    {
        await _roles.UpdateAsync(id, dto);
        return NoContent();
    }
}
