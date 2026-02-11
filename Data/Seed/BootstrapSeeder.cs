using CRM_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
            await _db.Database.MigrateAsync();

            // 1. DOMAIN
            var systemDomain = await _db.Domains
                .FirstOrDefaultAsync(d => d.DomainCode == "SYSTEM");

            if (systemDomain == null)
            {
                systemDomain = new CRM_Backend.Domain.Entities.Domain
                {
                    DomainCode = "SYSTEM",
                    DomainName = "System Root",
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Domains.Add(systemDomain);
                await _db.SaveChangesAsync();
            }

            // 2. ROLE
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
                    CreatedAt = DateTime.UtcNow
                };

                _db.Roles.Add(adminRole);
                await _db.SaveChangesAsync();
            }

            // 3. ADMIN USER
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
                    AccountStatus = "ACTIVE",
                    CreatedVia = "SYSTEM",
                    DomainId = systemDomain.DomainId,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(adminUser);
                await _db.SaveChangesAsync();

                // 4. PASSWORD
                _db.UserPasswords.Add(new UserPassword
                {
                    UserId = adminUser.UserId,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    IsCurrent = true,
                    CreatedAt = DateTime.UtcNow
                });

                // 5. SECURITY
                _db.UserSecurity.Add(new UserSecurity
                {
                    UserId = adminUser.UserId,
                    ForcePasswordReset = false,
                    FailedLoginCount = 0
                });

                // 6. ROLE ASSIGNMENT
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
