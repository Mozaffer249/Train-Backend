using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmailChange
{
    public class ConfirmEmailChangeCommandHandler : ResponseHandler, IRequestHandler<ConfirmEmailChangeCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityNotificationService _notificationService;

        public ConfirmEmailChangeCommandHandler(
            UserManager<User> userManager,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            IHttpContextAccessor httpContextAccessor,
            ISecurityNotificationService notificationService) : base(authLocalizer)
        {
            _userManager = userManager;
            _authLocalizer = authLocalizer;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
        }

        public async Task<Response<string>> Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return NotFound<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            var oldEmail = user.Email;

            // Change email using the token
            var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, request.Token);

            if (!result.Succeeded)
            {
                return BadRequest<string>(_authLocalizer[AuthenticationResourcesKeys.InvalidEmailChangeToken]);
            }

            // Also update the username to match the new email if it was the same as the old email
            if (user.UserName == oldEmail)
            {
                await _userManager.SetUserNameAsync(user, request.NewEmail);
            }

            // Mark email as confirmed
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Send security notification to OLD email
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            await _notificationService.NotifyEmailChangedAsync(oldEmail!, request.NewEmail, user.UserName!, ipAddress);

            return Success<string>(_authLocalizer[AuthenticationResourcesKeys.EmailChangedSuccessfully]);
        }
    }
}

