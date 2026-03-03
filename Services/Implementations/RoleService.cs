using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Roles;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

namespace CRM_Backend.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roles;
    private readonly IDomainRepository _domains;
    private readonly IModuleRepository _modules;

    public RoleService(
        IRoleRepository roles,
        IDomainRepository domains,
        IModuleRepository modules)
    {
        _roles = roles ?? throw new ArgumentNullException(nameof(roles));
        _domains = domains ?? throw new ArgumentNullException(nameof(domains));
        _modules = modules ?? throw new ArgumentNullException(nameof(modules));
    }

    private static RoleResponseDto Map(Role role)
    {
        return new RoleResponseDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            RoleCode = role.RoleCode,
            Description = role.Description,
            DomainId = role.DomainId,

            ModuleId = role.ModuleId,
            ModuleCode = role.Module?.ModuleCode,
            ModuleName = role.Module?.ModuleName,

            IsSystemRole = role.IsSystemRole,
            Active = role.Active
        };
    }

    public async Task<RoleResponseDto> CreateAsync(CreateRoleDto dto)
    {
        if (dto == null)
            throw new ValidationException("Role data is required.");

        if (string.IsNullOrWhiteSpace(dto.RoleName))
            throw new ValidationException("RoleName is required.");

        if (string.IsNullOrWhiteSpace(dto.RoleCode))
            throw new ValidationException("RoleCode is required.");

        if (string.IsNullOrWhiteSpace(dto.DomainCode))
            throw new ValidationException("DomainCode is required.");

        if (string.IsNullOrWhiteSpace(dto.ModuleCode))
            throw new ValidationException("ModuleCode is required.");

        var normalizedRoleCode = dto.RoleCode.Trim().ToUpper();
        var normalizedDomainCode = dto.DomainCode.Trim().ToUpper();
        var normalizedModuleCode = dto.ModuleCode.Trim().ToUpper();

        var domain = await _domains.GetByCodeAsync(normalizedDomainCode)
            ?? throw new NotFoundException($"Domain '{normalizedDomainCode}' not found.");

        var module = await _modules.GetByCodeAsync(normalizedModuleCode)
            ?? throw new NotFoundException($"Module '{normalizedModuleCode}' not found.");

        var isSystemRole = normalizedDomainCode == "SYSTEM";

        var role = await _roles.CreateAsync(
            dto.RoleName.Trim(),
            normalizedRoleCode,
            dto.Description?.Trim(),
            domain.DomainId,
            module.ModuleId,
            isSystemRole
        );

        return Map(role);
    }

    public async Task<List<RoleResponseDto>> GetAllAsync()
    {
        var roles = await _roles.GetAllAsync();
        return roles.Select(Map).ToList();
    }

    public async Task<List<RoleResponseDto>> GetByDomainAsync(string domainCode)
    {
        if (string.IsNullOrWhiteSpace(domainCode))
            throw new ValidationException("DomainCode is required.");

        var domain = await _domains.GetByCodeAsync(domainCode)
            ?? throw new NotFoundException($"Domain '{domainCode}' not found.");

        var roles = await _roles.GetByDomainIdAsync(domain.DomainId);

        return roles.Select(Map).ToList();
    }

    public async Task UpdateAsync(long id, UpdateRoleDto dto)
    {
        if (id <= 0)
            throw new ValidationException("Invalid role id.");

        if (dto == null)
            throw new ValidationException("Update data is required.");

        var role = await _roles.GetByIdAsync(id)
            ?? throw new NotFoundException($"Role {id} not found.");

        if (role.IsSystemRole)
            throw new BusinessRuleException("System roles cannot be modified.");

        role.Active = dto.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await _roles.UpdateAsync(role);
    }
}