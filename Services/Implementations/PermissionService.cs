using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Permissions;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

namespace CRM_Backend.Services.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissions;

    public PermissionService(IPermissionRepository permissions)
    {
        _permissions = permissions
            ?? throw new ArgumentNullException(nameof(permissions));
    }

    public async Task<long> CreateAsync(CreatePermissionDto dto)
    {
        if (dto == null)
            throw new ValidationException("Permission data is required.");

        if (string.IsNullOrWhiteSpace(dto.PermissionCode))
            throw new ValidationException("PermissionCode is required.");

        if (string.IsNullOrWhiteSpace(dto.Module))
            throw new ValidationException("Module is required.");

        var normalizedCode = dto.PermissionCode.Trim().ToUpper();
        var normalizedModule = dto.Module.Trim().ToUpper();

        var existing = await _permissions.GetByCodeAsync(normalizedCode);
        if (existing != null)
            throw new ConflictException(
                $"Permission '{normalizedCode}' already exists.");

        return await _permissions.CreateAsync(
            normalizedCode,
            dto.Description?.Trim(),
            normalizedModule
        );
    }

    public async Task<List<Permission>> GetAllAsync()
    {
        return await _permissions.GetAllAsync();
    }

    public async Task UpdateAsync(long id, UpdatePermissionDto dto)
    {
        if (id <= 0)
            throw new ValidationException("Invalid permission id.");

        if (dto == null)
            throw new ValidationException("Update data is required.");

        var permission = await _permissions.GetByIdAsync(id)
            ?? throw new NotFoundException($"Permission {id} not found.");

        // PermissionCode is immutable by design
        permission.Active = dto.IsActive;
        permission.UpdatedAt = DateTime.UtcNow;

        await _permissions.UpdateAsync(permission);
    }
}
