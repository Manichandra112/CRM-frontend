using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Repositories.Interfaces;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(long id);
    Task<Module?> GetByCodeAsync(string code);
    Task<List<Module>> GetAllAsync();
    Task<long> CreateAsync(string code, string name);
    Task UpdateAsync(Module module);
}