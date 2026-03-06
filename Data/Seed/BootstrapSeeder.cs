using System;
using System.Threading.Tasks;
using CRM_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CRM_Backend.Domain.Enums;

// Alias to avoid collision with namespace named "Domain"
using DomainEntity = CRM_Backend.Domain.Entities.Domain;

namespace CRM_Backend.Data.Seed
{
    public class BootstrapSeeder : IBootstrapSeeder
    {
        private readonly CrmAuthDbContext _db;
        private readonly IConfiguration _config;

        public BootstrapSeeder(CrmAuthDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task SeedAsync()
        {
            

            // --------------------------------------------------
            // 1️⃣ ENSURE DOMAIN (SYSTEM)
            // --------------------------------------------------
            var systemDomain = await _db.Domains
                .FirstOrDefaultAsync(d => d.DomainCode == "SYSTEM");

            if (systemDomain == null)
            {
                systemDomain = new DomainEntity
                {
                    DomainCode = "SYSTEM",
                    DomainName = "System Root",
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Domains.Add(systemDomain);
                await _db.SaveChangesAsync();
            }
            // --------------------------------------------------
            // 1️⃣ ENSURE MODULE (SYSTEM)
            // --------------------------------------------------
            var systemModule = await _db.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == "SYSTEM");

            if (systemModule == null)
            {
                systemModule = new Module
                {
                    ModuleCode = "SYSTEM",
                    ModuleName = "System Administration",
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Modules.Add(systemModule);
                await _db.SaveChangesAsync();
            }

            // --------------------------------------------------
            // 2️⃣ ENSURE ROLE (ADMIN)
            // --------------------------------------------------
            var adminRole = await _db.Roles
                .FirstOrDefaultAsync(r =>
                    r.RoleCode == "ADMIN" &&
                    r.DomainId == systemDomain.DomainId);

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    RoleName = "System Administrator",
                    RoleCode = "ADMIN",
                    Description = "Full system access",
                    Active = true,
                    IsSystemRole = true,
                    DomainId = systemDomain.DomainId,
                    ModuleId = systemModule.ModuleId, 
                    CreatedAt = DateTime.UtcNow
                };

                _db.Roles.Add(adminRole);
                await _db.SaveChangesAsync();
            }

            // --------------------------------------------------
            // 3️⃣ ENSURE PERMISSION (CRM_FULL_ACCESS)
            // --------------------------------------------------
            var fullAccessPermission = await _db.Permissions
                .FirstOrDefaultAsync(p => p.PermissionCode == "CRM_FULL_ACCESS");

            if (fullAccessPermission == null)
            {
                fullAccessPermission = new Permission
                {
                    PermissionCode = "CRM_FULL_ACCESS",
                    Description = "Full access to all CRM modules",
                    ModuleId = systemModule.ModuleId,
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Permissions.Add(fullAccessPermission);
                await _db.SaveChangesAsync();
            }

            // --------------------------------------------------
            // 4️⃣ ENSURE ROLE ↔ PERMISSION MAPPING
            // --------------------------------------------------
            var rolePermissionExists = await _db.RolePermissions
                .AnyAsync(rp =>
                    rp.RoleId == adminRole.RoleId &&
                    rp.PermissionId == fullAccessPermission.PermissionId);

            if (!rolePermissionExists)
            {
                _db.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.RoleId,
                    PermissionId = fullAccessPermission.PermissionId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = null
                });

                await _db.SaveChangesAsync();
            }

            // --------------------------------------------------
            // 5️⃣ ENSURE ADMIN USER
            // --------------------------------------------------
            var adminEmail = "admin@crm.com";
            var adminPassword = "Admin@123";

            var adminUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Username = "admin",
                    Email = adminEmail,
                    AccountStatus = AccountStatus.Active,
                    CreatedVia = "SYSTEM",
                    DomainId = systemDomain.DomainId,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(adminUser);
                await _db.SaveChangesAsync();
            }

            // --------------------------------------------------
            // 6️⃣ ENSURE PASSWORD
            // --------------------------------------------------
            var hasPassword = await _db.UserPasswords
                .AnyAsync(p => p.UserId == adminUser.UserId && p.IsCurrent);

            if (!hasPassword)
            {
                _db.UserPasswords.Add(new UserPassword
                {
                    UserId = adminUser.UserId,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    IsCurrent = true,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }

            // --------------------------------------------------
            // 7️⃣ ENSURE SECURITY ROW
            // --------------------------------------------------
            var hasSecurity = await _db.UserSecurity
                .AnyAsync(s => s.UserId == adminUser.UserId);

            if (!hasSecurity)
            {
                _db.UserSecurity.Add(new UserSecurity
                {
                    UserId = adminUser.UserId,
                    ForcePasswordReset = false,
                    FailedLoginCount = 0,
                    MfaEnabled = false
                });

                await _db.SaveChangesAsync();
            }

            // --------------------------------------------------
            // 8️⃣ ENSURE USER ↔ ROLE MAPPING
            // --------------------------------------------------
            var userRoleExists = await _db.UserRoles
                .AnyAsync(ur =>
                    ur.UserId == adminUser.UserId &&
                    ur.RoleId == adminRole.RoleId);

            if (!userRoleExists)
            {
                _db.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.RoleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = adminUser.UserId
                });

                await _db.SaveChangesAsync();
            }
        }
    }
}