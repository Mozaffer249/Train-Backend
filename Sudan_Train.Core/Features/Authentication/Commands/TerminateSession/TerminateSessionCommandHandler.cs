using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Commands.TerminateSession
{
    public class TerminateSessionCommandHandler : ResponseHandler, IRequestHandler<TerminateSessionCommand, Response<string>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly ISessionManagementService _sessionService;
        private readonly ISecurityNotificationService _notificationService;
        private readonly IOptions<SecuritySettings> _securitySettings;
        private readonly UserManager<User> _userManager;

        public TerminateSessionCommandHandler(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            ISessionManagementService sessionService,
            ISecurityNotificationService notificationService,
            IOptions<SecuritySettings> securitySettings,
            UserManager<User> userManager) : base(authLocalizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
            _sessionService = sessionService;
            _notificationService = notificationService;
            _securitySettings = securitySettings;
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(TerminateSessionCommand request, CancellationToken cancellationToken)
        {
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Get session details before terminating (for notification)
            var session = await _sessionService.GetSessionByIdAsync((int)request.SessionId, userId);

            if (session == null)
            {
                return NotFound<string>(_authLocalizer[AuthenticationResourcesKeys.SessionNotFound]);
            }

            // Terminate the session (the service will validate ownership)
            var result = await _sessionService.TerminateSessionAsync((int)request.SessionId, userId);

            if (!result)
            {
                return NotFound<string>(_authLocalizer[AuthenticationResourcesKeys.SessionNotFound]);
            }

            // Send email notification
            if (_securitySettings.Value.EmailNotifications.Enabled &&
                _securitySettings.Value.EmailNotifications.NotifyOnSessionTerminated)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    await _notificationService.NotifySessionTerminatedAsync(user, session.DeviceName);
                }
            }

            return Success<string>(_authLocalizer[AuthenticationResourcesKeys.SessionTerminated]);
        }
    }
}

