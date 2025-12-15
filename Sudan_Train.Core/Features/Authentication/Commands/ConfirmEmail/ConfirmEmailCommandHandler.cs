using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : ResponseHandler, IRequestHandler<ConfirmEmailCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public ConfirmEmailCommandHandler(
            IStringLocalizer<SharedResources> sharedLocalizer,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            UserManager<User> userManager) : base(sharedLocalizer)
        {
            _userManager = userManager;
            _authLocalizer = authLocalizer;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            // Find user
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                return NotFound<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Confirm email
            var result = await _userManager.ConfirmEmailAsync(user, request.Code);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            // Activate user account
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Success<string>("Email confirmed successfully. You can now login.");
        }
    }
}
