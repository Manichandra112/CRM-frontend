


using CRM_Backend.Domain.Enums;
using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Access;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

using DomainEntity = CRM_Backend.Domain.Entities.Domain;

namespace CRM_Backend.Services.Implementations
{
    public class AccessService : IAccessService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPermissionRepository _permissionRepo;
        private readonly IDomainRepository _domainRepo;

        public AccessService(
            IUserRepository userRepo,
            IUserRoleRepository userRoleRepo,
            IRoleRepository roleRepo,
            IPermissionRepository permissionRepo,
            IDomainRepository domainRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _userRoleRepo = userRoleRepo ?? throw new ArgumentNullException(nameof(userRoleRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
            _permissionRepo = permissionRepo ?? throw new ArgumentNullException(nameof(permissionRepo));
            _domainRepo = domainRepo ?? throw new ArgumentNullException(nameof(domainRepo));
        }

        public async Task<AccessMeResponse> GetMeAsync(long userId)
        {
            if (userId <= 0)
                throw new ValidationException("Invalid user id.");

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new NotFoundException("User not found.");

            if (user.AccountStatus != AccountStatus.Active)
                throw new UnauthorizedException("Account is not active.");

            var roleCodes =
                await _userRoleRepo.GetRoleCodesByUserIdAsync(userId)
                ?? Enumerable.Empty<string>();

            var roles = roleCodes.Any()
                ? await _roleRepo.GetByCodesAsync(roleCodes)
                : new List<Role>();

            var permissionCodes =
                await _userRoleRepo.GetPermissionCodesByUserIdAsync(userId)
                ?? Enumerable.Empty<string>();

            var permissions = permissionCodes.Any()
                ? await _permissionRepo.GetByCodesAsync(permissionCodes)
                : new List<Permission>();

            // 🔥 Fetch domains in ONE call instead of loop
            var moduleCodes = permissions
                .Select(p => p.Module)
                .Distinct()
                .ToList();

            var domainEntities = moduleCodes.Any()
                ? await _domainRepo.GetByCodesAsync(moduleCodes)
                : new List<DomainEntity>();

            var domainMap = domainEntities
                .ToDictionary(d => d.DomainCode, d => d);

            var domainDtos = permissions
                .GroupBy(p => p.Module)
                .Where(g => domainMap.ContainsKey(g.Key))
                .Select(g =>
                {
                    var domain = domainMap[g.Key];

                    return new AccessDomainDto
                    {
                        Code = domain.DomainCode,
                        Name = domain.DomainName,
                        Permissions = g.Select(p => new AccessPermissionDto
                        {
                            Code = p.PermissionCode,
                            Name = p.Description
                        }).ToList()
                    };
                })
                .ToList();

            return new AccessMeResponse
            {
                User = new AccessUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    AccountStatus = user.AccountStatus.ToString(),
                    PwdResetRequired = user.Security?.ForcePasswordReset ?? false
                },

                Roles = roles.Select(r => new AccessRoleDto
                {
                    Code = r.RoleCode,
                    Name = r.RoleName
                }).ToList(),

                Domains = domainDtos,

                PermissionsFlat = permissions
                    .Select(p => p.PermissionCode)
                    .Distinct()
                    .ToList()
            };
        }
    }
}
