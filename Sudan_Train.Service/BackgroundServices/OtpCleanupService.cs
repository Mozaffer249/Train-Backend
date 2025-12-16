using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Service.BackgroundServices
{
    public class OtpCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OtpCleanupService> _logger;

        public OtpCleanupService(IServiceProvider serviceProvider, ILogger<OtpCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OTP Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredOtpsAsync();
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Run every 10 minutes
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up expired OTPs");
                }
            }

            _logger.LogInformation("OTP Cleanup Service is stopping.");
        }

        private async Task CleanupExpiredOtpsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            // Clean email confirmation OTPs
            var expiredEmailOtps = context.EmailConfirmationOtps
                .Where(o => o.ExpiresAt < DateTime.UtcNow || o.IsUsed);
            var emailCount = expiredEmailOtps.Count();

            // Clean password reset OTPs
            var expiredPasswordOtps = context.PasswordResetOtps
                .Where(o => o.ExpiresAt < DateTime.UtcNow || o.IsUsed);
            var passwordCount = expiredPasswordOtps.Count();

            if (emailCount > 0 || passwordCount > 0)
            {
                context.EmailConfirmationOtps.RemoveRange(expiredEmailOtps);
                context.PasswordResetOtps.RemoveRange(expiredPasswordOtps);
                await context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {EmailCount} email OTPs and {PasswordCount} password reset OTPs",
                    emailCount, passwordCount);
            }
        }
    }
}
