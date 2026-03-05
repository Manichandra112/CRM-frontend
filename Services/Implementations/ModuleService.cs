using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Module;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

namespace CRM_Backend.Services.Implementations;

public class ModuleService : IModuleService
{
    private readonly IModuleRepository _moduleRepo;

    public ModuleService(IModuleRepository moduleRepo)
    {
        _moduleRepo = moduleRepo;
    }

    public async Task<List<Module>> GetAllAsync()
    {
        return await _moduleRepo.GetAllAsync();
    }

    public async Task<long> CreateAsync(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new Exception("Module code is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Module name is required.");

        return await _moduleRepo.CreateAsync(code, name);
    }

    public async Task<List<ModuleListDto>> GetAvailableAsync()
    {
        var modules = await _moduleRepo.GetAllAsync();

        return modules.Select(m => new ModuleListDto
        {
            ModuleId = m.ModuleId,
            ModuleCode = m.ModuleCode,
            ModuleName = m.ModuleName
        }).ToList();
    }
}