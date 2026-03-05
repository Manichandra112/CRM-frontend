using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Module;

namespace CRM_Backend.Services.Interfaces;

public interface IModuleService
{
    Task<List<Module>> GetAllAsync();               // detailed modules
    Task<List<ModuleListDto>> GetAvailableAsync();  // simple module list
    Task<long> CreateAsync(string code, string name);
}