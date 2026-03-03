using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly CrmAuthDbContext _context;

    public RoleRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByCodeAsync(string roleCode)
    {
        return await _context.Roles
            .Include(r => r.Module)
            .FirstOrDefaultAsync(r => r.RoleCode == roleCode);
    }

    public async Task<Role> CreateAsync(
        string roleName,
        string roleCode,
        string? description,
        long domainId,
        long moduleId,
        bool isSystemRole
    )
    {
        if (await _context.Roles
    .AnyAsync(r => r.RoleCode == roleCode && r.DomainId == domainId))
        {
            throw new Exception("Role already exists in this domain.");
        }

        var role = new Role
        {
            RoleName = roleName,
            RoleCode = roleCode,
            Description = description,
            DomainId = domainId,
            ModuleId = moduleId,
            IsSystemRole = isSystemRole,
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return role;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .Include(r => r.Module)
            .Where(r => r.Active)
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }

    public async Task<long> GetRoleIdByCodeAsync(string roleCode)
    {
        return await _context.Roles
            .Where(r => r.RoleCode == roleCode && r.Active)
            .Select(r => r.RoleId)
            .SingleAsync();
    }

    public async Task<long> GetRoleIdByCodeAndDomainAsync(string roleCode, long domainId)
    {
        return await _context.Roles
            .Where(r => r.RoleCode == roleCode
                     && r.DomainId == domainId
                     && r.Active)
            .Select(r => r.RoleId)
            .SingleAsync();
    }

    public async Task<List<Role>> GetByDomainIdAsync(long domainId)
    {
        return await _context.Roles
            .AsNoTracking()
            .Include(r => r.Module)
            .Where(r => r.DomainId == domainId && r.Active)
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(long id)
    {
        return await _context.Roles
            .Include(r => r.Module)
            .FirstOrDefaultAsync(r => r.RoleId == id);
    }

    public async Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Role>> GetByCodesAsync(IEnumerable<string> roleCodes)
    {
        return await _context.Roles
            .AsNoTracking()
            .Include(r => r.Module)
            .Where(r => roleCodes.Contains(r.RoleCode) && r.Active)
            .ToListAsync();
    }
}