using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : ResponseHandler, IRequestHandler<ResetPasswordCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public ResetPasswordCommandHandler(
            IStringLocalizer<SharedResources> sharedLocalizer,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            UserManager<User> userManager) : base(sharedLocalizer)
        {
            _userManager = userManager;
            _authLocalizer = authLocalizer;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound<string>(_authLocalizer[AuthenticationResourcesKeys.EmailIsNotExist]);
            }

            if (!user.IsActive)
            {
                return Unauthorized<string>(_authLocalizer[AuthenticationResourcesKeys.UserIsNotActive]);
            }

            // Reset password with token
            var result = await _userManager.ResetPasswordAsync(user, request.ResetCode, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            return Success<string>(_sharedLocalizer[SharedResourcesKeys.Updated]);
        }
    }
}
