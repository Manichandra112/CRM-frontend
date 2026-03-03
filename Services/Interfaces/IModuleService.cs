using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Services.Interfaces;

public interface IModuleService
{
    Task<List<Module>> GetAllAsync();
    Task<long> CreateAsync(string code, string name);
}   