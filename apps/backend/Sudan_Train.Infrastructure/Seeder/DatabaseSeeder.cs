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
                _logger.LogInformation("Starting database initialization...");

                // Check if database exists and can connect
                var canConnect = await _context.Database.CanConnectAsync();

                if (!canConnect)
                {
                    _logger.LogInformation("Database does not exist. It will be created with migrations.");
                }
                else
                {
                    _logger.LogInformation("Database connection successful.");

                    // Log applied migrations
                    var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
                    if (appliedMigrations.Any())
                    {
                        _logger.LogInformation($"Applied migrations: {appliedMigrations.Count()}");
                        foreach (var migration in appliedMigrations)
                        {
                            _logger.LogDebug($"  ✓ {migration}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No migrations have been applied yet.");
                    }
                }

                // Check for pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    _logger.LogInformation($"Found {pendingMigrations.Count()} pending migration(s):");
                    foreach (var migration in pendingMigrations)
                    {
                        _logger.LogInformation($"  → {migration}");
                    }

                    _logger.LogInformation("Applying migrations...");
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("✓ All migrations applied successfully!");
                }
                else
                {
                    _logger.LogInformation("✓ Database is up to date. No pending migrations.");
                }

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ An error occurred while initializing the database.");
                _logger.LogError($"Error details: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }

                // Check for specific errors and provide helpful guidance
                if (ex.Message.Contains("There is already an object named"))
                {
                    _logger.LogError("⚠️  Database has existing tables that conflict with the migration.");
                    _logger.LogError("📋 Solutions:");
                    _logger.LogError("   1. Drop the database and recreate it (recommended for development)");
                    _logger.LogError("   2. Run the cleanup_database.sql script to remove old tables");
                    _logger.LogError("   3. Manually remove conflicting tables from the database");
                }
                else if (ex.Message.Contains("network-related") || ex.Message.Contains("not found or was not accessible"))
                {
                    _logger.LogError("⚠️  Cannot connect to the database server.");
                    _logger.LogError("📋 Check:");
                    _logger.LogError("   1. Connection string is correct in appsettings.Development.json");
                    _logger.LogError("   2. Database server is accessible and running");
                    _logger.LogError("   3. Firewall allows the connection");
                    _logger.LogError("   4. Server name and credentials are correct");
                }

                throw;
            }
        }
    }
}

