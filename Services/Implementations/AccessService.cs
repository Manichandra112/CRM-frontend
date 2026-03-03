
//using CRM_Backend.Domain.Enums;
//using CRM_Backend.Domain.Entities;
//using CRM_Backend.DTOs.Access;
//using CRM_Backend.Exceptions;
//using CRM_Backend.Repositories.Interfaces;
//using CRM_Backend.Services.Interfaces;


//namespace CRM_Backend.Services.Implementations
//{
//    public class AccessService : IAccessService
//    {
//        private readonly IUserRepository _userRepo;
//        private readonly IUserRoleRepository _userRoleRepo;
//        private readonly IRoleRepository _roleRepo;
//        private readonly IPermissionRepository _permissionRepo;

//        public AccessService(
//            IUserRepository userRepo,
//            IUserRoleRepository userRoleRepo,
//            IRoleRepository roleRepo,
//            IPermissionRepository permissionRepo)
//        {
//            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
//            _userRoleRepo = userRoleRepo ?? throw new ArgumentNullException(nameof(userRoleRepo));
//            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
//            _permissionRepo = permissionRepo ?? throw new ArgumentNullException(nameof(permissionRepo));

//        }

//        public async Task<AccessMeResponse> GetMeAsync(long userId)
//        {
//            if (userId <= 0)
//                throw new ValidationException("Invalid user id.");

//            var user = await _userRepo.GetByIdAsync(userId)
//                ?? throw new NotFoundException("User not found.");

//            if (user.AccountStatus != AccountStatus.Active)
//                throw new UnauthorizedException("Account is not active.");

//            var roleCodes =
//                await _userRoleRepo.GetRoleCodesByUserIdAsync(userId)
//                ?? Enumerable.Empty<string>();

//            var roles = roleCodes.Any()
//                ? await _roleRepo.GetByCodesAsync(roleCodes)
//                : new List<Role>();

//            var permissionCodes =
//                await _userRoleRepo.GetPermissionCodesByUserIdAsync(userId)
//                ?? Enumerable.Empty<string>();

//            var permissions = permissionCodes.Any()
//                ? await _permissionRepo.GetByCodesAsync(permissionCodes)
//                : new List<Permission>();

//            // 🔥 Fetch domains in ONE call instead of loop

//            var domainDtos = permissions
//    .GroupBy(p => p.Module.ModuleId)
//    .Select(g =>
//    {
//        var module = g.First().Module;

//        return new AccessDomainDto
//        {
//            Code = module.ModuleCode,
//            Name = module.ModuleName,
//            Permissions = g.Select(p => new AccessPermissionDto
//            {
//                Code = p.PermissionCode,
//                Name = p.Description
//            }).ToList()
//        };
//    })
//    .ToList();






//            return new AccessMeResponse
//            {
//                User = new AccessUserDto
//                {
//                    Id = user.UserId,
//                    Username = user.Username,
//                    Email = user.Email,
//                    AccountStatus = user.AccountStatus.ToString(),
//                    PwdResetRequired = user.Security?.ForcePasswordReset ?? false
//                },

//                Roles = roles.Select(r => new AccessRoleDto
//                {
//                    Code = r.RoleCode,
//                    Name = r.RoleName
//                }).ToList(),

//                Domains = domainDtos,

//                PermissionsFlat = permissions
//                    .Select(p => p.PermissionCode)
//                    .Distinct()
//                    .ToList()
//            };
//        }
//    }
//}

using CRM_Backend.Domain.Enums;
using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Access;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

namespace CRM_Backend.Services.Implementations
{
    public class AccessService : IAccessService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPermissionRepository _permissionRepo;

        public AccessService(
            IUserRepository userRepo,
            IUserRoleRepository userRoleRepo,
            IRoleRepository roleRepo,
            IPermissionRepository permissionRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _userRoleRepo = userRoleRepo ?? throw new ArgumentNullException(nameof(userRoleRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
            _permissionRepo = permissionRepo ?? throw new ArgumentNullException(nameof(permissionRepo));
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

            // -----------------------------
            // SAFE MODULE GROUPING
            // -----------------------------
            var moduleDtos = permissions
                .Where(p => p.ModuleId != null && p.Module != null)
                .GroupBy(p => p.ModuleId)
                .Select(g =>
                {
                    var module = g.First().Module!;

                    return new AccessDomainDto
                    {
                        Code = module.ModuleCode,
                        Name = module.ModuleName,
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

                Domains = moduleDtos,

                PermissionsFlat = permissions
                    .Select(p => p.PermissionCode)
                    .Distinct()
                    .ToList()
            };
        }
    }
}