using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;

namespace Sudan_Train.Service.BackgroundServices
{
    public class SessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly SecuritySettings _settings;

        public SessionCleanupService(
            IServiceProvider serviceProvider,
            ILogger<SessionCleanupService> logger,
            IOptions<SecuritySettings> settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_settings.SessionTimeout.Enabled)
            {
                _logger.LogInformation("Session timeout is disabled. SessionCleanupService will not run.");
                return;
            }

            _logger.LogInformation("SessionCleanupService is starting. Checking every {Minutes} minutes for sessions inactive for {InactivityMinutes} minutes.",
                _settings.SessionTimeout.CheckIntervalMinutes,
                _settings.SessionTimeout.InactivityMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupInactiveSessionsAsync();
                    await Task.Delay(TimeSpan.FromMinutes(_settings.SessionTimeout.CheckIntervalMinutes), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up inactive sessions");
                }
            }

            _logger.LogInformation("SessionCleanupService is stopping.");
        }

        private async Task CleanupInactiveSessionsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<ISecurityNotificationService>();

            var cutoffTime = DateTime.UtcNow.AddMinutes(-_settings.SessionTimeout.InactivityMinutes);

            var inactiveSessions = await context.LoginSessions
                .Where(s => s.IsActive && s.LastActivityTime < cutoffTime)
                .Include(s => s.User)
                .ToListAsync();

            if (!inactiveSessions.Any())
            {
                _logger.LogDebug("No inactive sessions found to clean up.");
                return;
            }

            foreach (var session in inactiveSessions)
            {
                session.IsActive = false;
                session.LogoutTime = DateTime.UtcNow;

                // Optionally notify user
                if (_settings.EmailNotifications.Enabled &&
                    _settings.EmailNotifications.NotifyOnSessionTerminated)
                {
                    try
                    {
                        await notificationService.NotifySessionTerminatedAsync(
                            session.User,
                            $"{session.DeviceName} (auto-logout due to inactivity)");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send session termination notification for user {UserId}", session.UserId);
                    }
                }
            }

            await context.SaveChangesAsync();
            _logger.LogInformation("Terminated {Count} inactive sessions (inactive for more than {Minutes} minutes)",
                inactiveSessions.Count,
                _settings.SessionTimeout.InactivityMinutes);
        }
    }
}
