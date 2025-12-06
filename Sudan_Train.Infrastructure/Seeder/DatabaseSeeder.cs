using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Sudan_Train.Infrastructure.context;
using System;
using System.Threading.Tasks;

namespace Sudan_Train.Infrastructure.Seeder
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(ApplicationDBContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var databaseCreator = _context.GetService<IRelationalDatabaseCreator>() as RelationalDatabaseCreator;

                if (databaseCreator != null)
                {
                    // Check if can connect to database
                    if (!databaseCreator.CanConnect())
                    {
                        _logger.LogInformation("Creating database...");
                        databaseCreator.Create();
                        _logger.LogInformation("Database created successfully.");
                    }

                    // Check if database has tables
                    if (!databaseCreator.HasTables())
                    {
                        _logger.LogInformation("Creating database tables...");
                        databaseCreator.CreateTables();
                        _logger.LogInformation("Database tables created successfully.");
                    }
                }

                // Apply any pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying pending migrations...");
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("Migrations applied successfully.");
                }

                _logger.LogInformation("Database initialization completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}

