using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Infrastructure.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<User> _userManager)
        {
            var usersCount = await _userManager.Users.CountAsync();
            if (usersCount <= 0)
            {
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
            }
        }
    }
}