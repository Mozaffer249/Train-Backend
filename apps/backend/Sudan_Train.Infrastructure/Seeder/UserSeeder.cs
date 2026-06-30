using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.Entity.Identity;
using System;
using System.Threading.Tasks;

namespace Sudan_Train.Infrastructure.Seeder
{
    public class UserSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(UserManager<User> userManager, ILogger<UserSeeder> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Checking and seeding default users...");

                // Seed SuperAdmin user
                await CreateSuperAdminUserAsync();

                // Seed default Admin user
                await CreateAdminUserAsync();

                _logger.LogInformation("User seeding completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding users.");
                throw;
            }
        }

        private async Task CreateSuperAdminUserAsync()
        {
            const string superAdminEmail = "superadmin@sudantrain.com";
            const string superAdminUsername = "superadmin";
            const string superAdminPassword = "SuperAdmin@123";

            var existingUser = await _userManager.FindByEmailAsync(superAdminEmail);
            if (existingUser != null)
            {
                // Ensure the user has SuperAdmin role
                if (!await _userManager.IsInRoleAsync(existingUser, Roles.SuperAdmin))
                {
                    await _userManager.AddToRoleAsync(existingUser, Roles.SuperAdmin);
                    _logger.LogInformation("Added SuperAdmin role to existing user '{Email}'.", superAdminEmail);
                }
                _logger.LogDebug("SuperAdmin user already exists.");
                return;
            }

            var superAdminUser = new User()
            {
                UserName = superAdminUsername,
                Email = superAdminEmail,
                FirstName = "Super",
                LastName = "Admin",
                Address = "Sudan",
                Nationality = "Sudanese",
                Code = "SA001",
                IsActive = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = "+249123456789",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                LockoutEnd = null,
                TwoFactorEnabled = false,
                PasswordChangedAt = DateTime.UtcNow,
                MustChangePassword = false,
            };

            var createResult = await _userManager.CreateAsync(superAdminUser, superAdminPassword);
            if (createResult.Succeeded)
            {
                // Assign SuperAdmin role (which inherently has all permissions)
                await _userManager.AddToRoleAsync(superAdminUser, Roles.SuperAdmin);

                _logger.LogInformation("SuperAdmin user created successfully.");
                _logger.LogWarning("=== DEFAULT SUPERADMIN CREDENTIALS ===");
                _logger.LogWarning("Email: {Email}", superAdminEmail);
                _logger.LogWarning("Password: {Password}", superAdminPassword);
                _logger.LogWarning("IMPORTANT: Change these credentials immediately in production!");
                _logger.LogWarning("==========================================");
            }
            else
            {
                _logger.LogError("Failed to create SuperAdmin user: {Errors}",
                    string.Join(", ", createResult.Errors));
            }
        }

        private async Task CreateAdminUserAsync()
        {
            const string adminEmail = "admin@sudantrain.com";
            const string adminUsername = "admin";
            const string adminPassword = "Admin@123";

            var existingUser = await _userManager.FindByEmailAsync(adminEmail);
            if (existingUser != null)
            {
                // Ensure the user has the Admin role.
                if (!await _userManager.IsInRoleAsync(existingUser, Roles.Admin))
                {
                    await _userManager.AddToRoleAsync(existingUser, Roles.Admin);
                    _logger.LogInformation("Added Admin role to existing user '{Email}'.", adminEmail);
                }
                _logger.LogDebug("Admin user already exists.");
                return;
            }

            var adminUser = new User()
            {
                UserName = adminUsername,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                Address = "Sudan",
                Nationality = "Sudanese",
                Code = "AD001",
                IsActive = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = "+249123456788",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                LockoutEnd = null,
                TwoFactorEnabled = false,
                PasswordChangedAt = DateTime.UtcNow,
                MustChangePassword = false,
            };

            var createResult = await _userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, Roles.Admin);

                _logger.LogInformation("Admin user created successfully.");
                _logger.LogWarning("=== DEFAULT ADMIN CREDENTIALS ===");
                _logger.LogWarning("Email: {Email}", adminEmail);
                _logger.LogWarning("Password: {Password}", adminPassword);
                _logger.LogWarning("IMPORTANT: Change these credentials immediately in production!");
                _logger.LogWarning("=================================");
            }
            else
            {
                _logger.LogError("Failed to create Admin user: {Errors}",
                    string.Join(", ", createResult.Errors));
            }
        }
    }
}