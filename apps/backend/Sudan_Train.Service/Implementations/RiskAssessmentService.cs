using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;

namespace Sudan_Train.Service.Implementations
{
    public class RiskAssessmentService : IRiskAssessmentService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMemoryCache _cache;
        private readonly IOptions<SecuritySettings> _securitySettings;
        private const string BLOCKED_IP_PREFIX = "BlockedIP_";

        public RiskAssessmentService(
            ApplicationDBContext context,
            IMemoryCache cache,
            IOptions<SecuritySettings> securitySettings)
        {
            _context = context;
            _cache = cache;
            _securitySettings = securitySettings;
        }

        public async Task<RiskAssessment> AssessLoginRiskAsync(string ipAddress, int? userId)
        {
            var settings = _securitySettings.Value.RiskBasedAuth;

            if (!settings.Enabled)
            {
                return new RiskAssessment
                {
                    Level = RiskLevel.Low,
                    RequiresTwoFactor = false,
                    Reason = "Risk-based authentication is disabled"
                };
            }

            // Check if IP is blocked
            if (await IsIpBlockedAsync(ipAddress))
            {
                return new RiskAssessment
                {
                    Level = RiskLevel.Critical,
                    RequiresTwoFactor = false,
                    Reason = "IP address is currently blocked",
                    FailedAttemptsInWindow = 0
                };
            }

            // Calculate time window
            var windowStart = DateTime.UtcNow.AddMinutes(-settings.FailedAttemptWindowMinutes);

            // IP login-attempt tracking disabled — IpLoginAttempt table dropped (DropAdvancedSecurityTables migration).
            var failedAttempts = 0;
            //var failedAttempts = await _context.IpLoginAttempts
            //    .Where(a => a.IpAddress == ipAddress &&
            //               !a.WasSuccessful &&
            //               a.AttemptTime >= windowStart)
            //    .CountAsync();

            // Assess risk level
            var assessment = new RiskAssessment
            {
                FailedAttemptsInWindow = failedAttempts
            };

            if (failedAttempts >= settings.MaxFailedAttemptsPerIp)
            {
                assessment.Level = RiskLevel.Critical;
                assessment.RequiresTwoFactor = false;
                assessment.Reason = $"{failedAttempts} failed login attempts detected";
            }
            else if (failedAttempts >= (settings.MaxFailedAttemptsPerIp * 0.7)) // 70% threshold
            {
                assessment.Level = RiskLevel.High;
                assessment.RequiresTwoFactor = settings.RequireTwoFactorForSuspiciousIp;
                assessment.Reason = $"{failedAttempts} failed login attempts detected";
            }
            else if (failedAttempts >= (settings.MaxFailedAttemptsPerIp * 0.4)) // 40% threshold
            {
                assessment.Level = RiskLevel.Medium;
                assessment.RequiresTwoFactor = false;
                assessment.Reason = $"{failedAttempts} failed login attempts detected";
            }
            else
            {
                assessment.Level = RiskLevel.Low;
                assessment.RequiresTwoFactor = false;
                assessment.Reason = "No suspicious activity detected";
            }

            return assessment;
        }

        public async Task RecordLoginAttemptAsync(string ipAddress, int? userId, string? userName, bool wasSuccessful)
        {
            // IP login-attempt tracking disabled — IpLoginAttempt table dropped (DropAdvancedSecurityTables migration).
            await Task.CompletedTask;

            /* Original implementation preserved for restoration:
            var attempt = new IpLoginAttempt
            {
                IpAddress = ipAddress,
                UserId = userId,
                UserName = userName,
                WasSuccessful = wasSuccessful,
                AttemptTime = DateTime.UtcNow
            };

            _context.IpLoginAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            */
        }

        public async Task<bool> IsIpBlockedAsync(string ipAddress)
        {
            var cacheKey = $"{BLOCKED_IP_PREFIX}{ipAddress}";
            return await Task.FromResult(_cache.TryGetValue(cacheKey, out _));
        }

        public async Task BlockIpAsync(string ipAddress, int durationMinutes)
        {
            var cacheKey = $"{BLOCKED_IP_PREFIX}{ipAddress}";
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(durationMinutes)
            };

            _cache.Set(cacheKey, true, options);
            await Task.CompletedTask;
        }
    }
}
