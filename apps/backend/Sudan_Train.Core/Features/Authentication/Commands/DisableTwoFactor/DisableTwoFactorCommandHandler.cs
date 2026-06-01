using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Abstracts;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Commands.DisableTwoFactor
{
    public class DisableTwoFactorCommandHandler : ResponseHandler, IRequestHandler<DisableTwoFactorCommand, Response<string>>
    {
        private readonly ITwoFactorAuthenticationService _twoFactorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly ISecurityNotificationService _notificationService;
        private readonly UserManager<User> _userManager;

        public DisableTwoFactorCommandHandler(
            ITwoFactorAuthenticationService twoFactorService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            ISecurityNotificationService notificationService,
            UserManager<User> userManager) : base(authLocalizer)
        {
            _twoFactorService = twoFactorService;
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            // Two-factor authentication temporarily disabled — TwoFactorRecoveryCode table dropped (DropAdvancedSecurityTables migration).
            await Task.CompletedTask;
            return BadRequest<string>("Two-factor authentication is temporarily disabled.");

            /* Original implementation preserved for restoration:
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            var result = await _twoFactorService.DisableTwoFactorAsync(userId, request.Password);

            if (!result)
            {
                return BadRequest<string>(_authLocalizer[AuthenticationResourcesKeys.PasswordNotCorrect]);
            }

            // Send security notification
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                await _notificationService.NotifyTwoFactorDisabledAsync(user);
            }

            return Success<string>("Two-factor authentication disabled successfully");
            */
        }
    }
}
