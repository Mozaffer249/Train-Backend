using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sudan_Train.Data.Entity.Identity;
using System;
using System.Threading.Tasks;

namespace Sudan_Train.Infrastructure.Seeder
{
	public class RoleSeeder
	{
		private readonly RoleManager<Role> _roleManager;
		private readonly ILogger<RoleSeeder> _logger;

		public RoleSeeder(RoleManager<Role> roleManager, ILogger<RoleSeeder> logger)
		{
			_roleManager = roleManager;
			_logger = logger;
		}

		public async Task SeedAsync()
		{
			try
			{
				var rolesCount = await _roleManager.Roles.CountAsync();
				if (rolesCount <= 0)
				{
					_logger.LogInformation("Seeding roles...");

					await _roleManager.CreateAsync(new Role()
					{
						Name = "Admin",
						NormalizedName = "ADMIN",
						ConcurrencyStamp = Guid.NewGuid().ToString()
					});

					await _roleManager.CreateAsync(new Role()
					{
						Name = "User",
						NormalizedName = "USER",
						ConcurrencyStamp = Guid.NewGuid().ToString()
					});

					_logger.LogInformation("Roles seeded successfully.");
				}
				else
				{
					_logger.LogInformation("Roles already exist. Skipping seeding.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while seeding roles.");
				throw;
			}
		}
	}
}