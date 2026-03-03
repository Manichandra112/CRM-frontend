using CRM_Backend.Services.Interfaces;
using CRM_Backend.DTOs.Module;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers.Admin;

[ApiController]
[Route("api/modules")]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _service;

    public ModulesController(IModuleService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateModuleDto dto)
    {
        var id = await _service.CreateAsync(dto.ModuleCode, dto.ModuleName);
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
}