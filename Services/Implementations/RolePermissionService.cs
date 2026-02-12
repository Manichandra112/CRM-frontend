namespace CRM_Backend.Services.Implementations;

using CRM_Backend.DTOs.Roles;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _repo;
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;

    public RolePermissionService(
        IRolePermissionRepository repo,
        IRoleRepository roles,
        IPermissionRepository permissions)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _roles = roles ?? throw new ArgumentNullException(nameof(roles));
        _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
    }

    public async Task AssignPermissionAsync(
        string roleCode,
        string permissionCode,
        long adminId)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            throw new ValidationException("RoleCode is required.");

        if (string.IsNullOrWhiteSpace(permissionCode))
            throw new ValidationException("PermissionCode is required.");

        var normalizedRoleCode = roleCode.Trim().ToUpper();
        var normalizedPermissionCode = permissionCode.Trim().ToUpper();

        var role = await _roles.GetByCodeAsync(normalizedRoleCode)
            ?? throw new NotFoundException($"Role '{normalizedRoleCode}' not found.");

        var permissionId =
            await _permissions.GetPermissionIdByCodeAsync(normalizedPermissionCode);

        if (permissionId == null)
            throw new NotFoundException($"Permission '{normalizedPermissionCode}' not found.");

        var alreadyAssigned =
            await _repo.ExistsAsync(role.RoleId, permissionId.Value);

        if (alreadyAssigned)
            throw new ConflictException(
                $"Permission '{normalizedPermissionCode}' is already assigned to role '{normalizedRoleCode}'.");

        await _repo.AssignPermissionAsync(
            role.RoleId,
            permissionId.Value,
            adminId);
    }

    public async Task RemovePermissionAsync(
        string roleCode,
        string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            throw new ValidationException("RoleCode is required.");

        if (string.IsNullOrWhiteSpace(permissionCode))
            throw new ValidationException("PermissionCode is required.");

        var normalizedRoleCode = roleCode.Trim().ToUpper();
        var normalizedPermissionCode = permissionCode.Trim().ToUpper();

        var role = await _roles.GetByCodeAsync(normalizedRoleCode)
            ?? throw new NotFoundException($"Role '{normalizedRoleCode}' not found.");

        var permissionId =
            await _permissions.GetPermissionIdByCodeAsync(normalizedPermissionCode);

        if (permissionId == null)
            throw new NotFoundException($"Permission '{normalizedPermissionCode}' not found.");

        var exists =
            await _repo.ExistsAsync(role.RoleId, permissionId.Value);

        if (!exists)
            throw new BusinessRuleException(
                $"Permission '{normalizedPermissionCode}' is not assigned to role '{normalizedRoleCode}'.");

        await _repo.RemovePermissionAsync(
            role.RoleId,
            permissionId.Value);
    }

    public async Task<IEnumerable<long>> GetPermissionIdsByRoleAsync(long roleId)
    {
        if (roleId <= 0)
            throw new ValidationException("Invalid role id.");

        return await _repo.GetPermissionIdsByRoleAsync(roleId);
    }

    public async Task<RolePermissionsResponseDto> GetRolePermissionsAsync(
        string roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            throw new ValidationException("RoleCode is required.");

        var normalizedRoleCode = roleCode.Trim().ToUpper();

        var role = await _roles.GetByCodeAsync(normalizedRoleCode)
            ?? throw new NotFoundException($"Role '{normalizedRoleCode}' not found.");

        var permissionIds =
            await _repo.GetPermissionIdsByRoleAsync(role.RoleId);

        var permissionCodes =
            await _permissions.GetPermissionCodesByIdsAsync(permissionIds);

        return new RolePermissionsResponseDto
        {
            RoleCode = normalizedRoleCode,
            Permissions = permissionCodes
        };
    }
}
