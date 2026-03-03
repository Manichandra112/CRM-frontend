using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class ModuleRepository : IModuleRepository
{
    private readonly CrmAuthDbContext _context;

    public ModuleRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<Module?> GetByIdAsync(long id)
    {
        return await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleId == id && m.Active);
    }

    public async Task<Module?> GetByCodeAsync(string code)
    {
        return await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleCode == code && m.Active);
    }

    public async Task<List<Module>> GetAllAsync()
    {
        return await _context.Modules
            .Include(m => m.Roles)
            .Include(m => m.Permissions)
            .Where(m => m.Active)
            .OrderBy(m => m.ModuleCode)
            .ToListAsync();
    }

    public async Task<long> CreateAsync(string code, string name)
    {
        if (await _context.Modules.AnyAsync(m => m.ModuleCode == code))
            throw new Exception("Module already exists.");

        var module = new Module
        {
            ModuleCode = code.ToUpper(),
            ModuleName = name,
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Modules.Add(module);
        await _context.SaveChangesAsync();

        return module.ModuleId;
    }

    public async Task UpdateAsync(Module module)
    {
        _context.Entry(module).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}