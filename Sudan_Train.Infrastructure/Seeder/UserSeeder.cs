using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
                var usersCount = await _userManager.Users.CountAsync();
                if (usersCount <= 0)
                {
                    _logger.LogInformation("Seeding default admin user...");

                    var defaultuser = new User()
                    {
                        UserName = "admin",
                        Email = "admin@project.com",
                        FirstName = "admin",
                        LastName = "admin",
                        Address = "Sudan",
                        Nationality = "Sudan",
                        Code = "123456",
                        IsActive = true,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        SecurityStamp = Guid.NewGuid().ToString(),
                        LockoutEnabled = false,
                        LockoutEnd = null,
                        TwoFactorEnabled = false,
                    };

                    await _userManager.CreateAsync(defaultuser, "Admin@123");
                    await _userManager.AddToRoleAsync(defaultuser, "Admin");

                    _logger.LogInformation("Default admin user created successfully.");
                    _logger.LogInformation("Username: admin, Password: Admin@123");
                }
                else
                {
                    _logger.LogInformation("Users already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding users.");
                throw;
            }
        }
    }
}