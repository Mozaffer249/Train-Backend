using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Authentication.Commands.Login;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Data.Results;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;
using System;

namespace Trains.Core.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : ResponseHandler, IRequestHandler<LoginCommand, Response<JwtAuthResult>>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly ISessionManagementService _sessionService;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityNotificationService _notificationService;
        private readonly IOptions<SecuritySettings> _securitySettings;
        private readonly IRiskAssessmentService _riskAssessmentService;

        public LoginCommandHandler(
            IStringLocalizer<AuthenticationResources> authLocalizer,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthenticationService authenticationService,
            ISessionManagementService sessionService,
            IAuditService auditService,
            IHttpContextAccessor httpContextAccessor,
            ISecurityNotificationService notificationService,
            IOptions<SecuritySettings> securitySettings,
            IRiskAssessmentService riskAssessmentService) : base(authLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
            _authLocalizer = authLocalizer;
            _sessionService = sessionService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _securitySettings = securitySettings;
            _riskAssessmentService = riskAssessmentService;
        }

        public async Task<Response<JwtAuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Get IP address early for risk assessment
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = GetClientIpAddress(httpContext);

            // Assess login risk before password check
            var riskAssessment = await _riskAssessmentService.AssessLoginRiskAsync(ipAddress, null);

            if (riskAssessment.Level == RiskLevel.Critical)
            {
                // Block the login attempt
                await _riskAssessmentService.BlockIpAsync(ipAddress, _securitySettings.Value.RiskBasedAuth.BlockSuspiciousIpMinutes);
                return Unauthorized<JwtAuthResult>("Access temporarily blocked due to suspicious activity. Please try again later.");
            }

            // Check if user exists (validation ensures UserName is not null)
            var user = await _userManager.FindByNameAsync(request.UserName!);
            if (user == null)
            {
                // Record failed attempt
                await _riskAssessmentService.RecordLoginAttemptAsync(ipAddress, null, request.UserName, false);
                return NotFound<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserIsNotActive]);
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.EmailNotConfirmed]);
            }

            // Try to sign in with lockout enabled (validation ensures Password is not null)
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, true);

            // Check if account is locked out
            if (signInResult.IsLockedOut)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.AccountLockedOut]);
            }

            // Check if 2FA is required
            if (signInResult.RequiresTwoFactor || user.TwoFactorEnabled)
            {
                return BadRequest<JwtAuthResult>("Two-factor authentication is required. Please use LoginWithTwoFactor endpoint.");
            }

            if (!signInResult.Succeeded)
            {
                // Record failed login attempt
                await _riskAssessmentService.RecordLoginAttemptAsync(ipAddress, user.Id, user.UserName, false);

                // Log suspicious activity if high risk
                if (riskAssessment.Level >= RiskLevel.High)
                {
                    await _auditService.LogSecurityEventAsync(
                        userId: user.Id,
                        eventType: SecurityEventType.SuspiciousActivity,
                        ipAddress: ipAddress,
                        details: $"Multiple failed login attempts: {riskAssessment.FailedAttemptsInWindow} in time window"
                    );

                    // Notify user of suspicious activity
                    if (_securitySettings.Value.EmailNotifications.Enabled &&
                        _securitySettings.Value.EmailNotifications.NotifyOnSuspiciousActivity)
                    {
                        await _notificationService.NotifySuspiciousActivityAsync(
                            user,
                            $"Multiple failed login attempts ({riskAssessment.FailedAttemptsInWindow})",
                            ipAddress);
                    }
                }

                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.PasswordNotCorrect]);
            }

            // Record successful login attempt
            await _riskAssessmentService.RecordLoginAttemptAsync(ipAddress, user.Id, user.UserName, true);

            // Check if 2FA is required due to risk assessment
            if (riskAssessment.RequiresTwoFactor && !user.TwoFactorEnabled)
            {
                // For now, just warn the user - in production, you'd force 2FA setup
                await _auditService.LogSecurityEventAsync(
                    userId: user.Id,
                    eventType: SecurityEventType.SuspiciousActivity,
                    ipAddress: ipAddress,
                    details: $"High-risk login detected. Recommendation: Enable 2FA. Failed attempts in window: {riskAssessment.FailedAttemptsInWindow}"
                );
            }

            // Generate JWT token
            var result = await _authenticationService.GetJWTToken(user);

            // Get device and IP info
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            var deviceId = request.DeviceId ?? Guid.NewGuid().ToString();
            var deviceName = request.DeviceName ?? ParseDeviceFromUserAgent(userAgent);

            // Check if device is trusted
            var isTrustedDevice = await _sessionService.IsDeviceTrustedAsync(user.Id, deviceId);

            // Create login session
            await _sessionService.CreateSessionAsync(
                userId: user.Id,
                deviceId: deviceId,
                deviceName: deviceName,
                ipAddress: ipAddress,
                userAgent: userAgent,
                accessToken: result.AccessToken,
                refreshToken: result.RefreshToken.TokenString,
                location: null // Optional: Use IP geolocation API
            );

            // Log security event if new device
            if (!isTrustedDevice)
            {
                await _auditService.LogSecurityEventAsync(
                    userId: user.Id,
                    eventType: SecurityEventType.LoginFromNewDevice,
                    ipAddress: ipAddress,
                    details: $"Login from new device: {deviceName}"
                );

                // Send email notification for new device login
                if (_securitySettings.Value.EmailNotifications.Enabled &&
                    _securitySettings.Value.EmailNotifications.NotifyOnNewDeviceLogin)
                {
                    await _notificationService.NotifyNewDeviceLoginAsync(user, deviceName, ipAddress);
                }
            }

            // Add device info to response for frontend to prompt "Trust this device?"
            result.IsNewDevice = !isTrustedDevice;
            result.DeviceId = deviceId;

            return Success(entity: result);
        }

        private string GetClientIpAddress(HttpContext? context)
        {
            if (context == null) return "Unknown";

            var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private string ParseDeviceFromUserAgent(string userAgent)
        {
            // Simple parser - can be enhanced
            if (userAgent.Contains("iPhone")) return "iPhone";
            if (userAgent.Contains("Android")) return "Android Device";
            if (userAgent.Contains("Windows")) return "Windows PC";
            if (userAgent.Contains("Mac")) return "Mac";
            return "Unknown Device";
        }
    }
}

