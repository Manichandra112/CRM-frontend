


using CRM_Backend.DTOs.Permissions;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/permissions")]

[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

public class AdminPermissionsController : ControllerBase
{
    private readonly IPermissionService _permissions;

    public AdminPermissionsController(IPermissionService permissions)
    {
        _permissions = permissions;
    }

    // CREATE NEW PERMISSION
    [HttpPost]
    [HasPermission("PERMISSION_ASSIGN")]
    public async Task<IActionResult> Create(CreatePermissionDto dto)
    {
        var id = await _permissions.CreateAsync(dto);
        return Ok(new { permissionId = id });
    }

    // VIEW ALL PERMISSIONS
    [HttpGet]
    [HasPermission("PERMISSION_VIEW")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _permissions.GetAllAsync());
    }

    // UPDATE PERMISSION
    [HttpPut("{id:long}")]
    [HasPermission("PERMISSION_ASSIGN")]
    public async Task<IActionResult> Update(long id, UpdatePermissionDto dto)
    {
        await _permissions.UpdateAsync(id, dto);
        return NoContent();
    }
}

