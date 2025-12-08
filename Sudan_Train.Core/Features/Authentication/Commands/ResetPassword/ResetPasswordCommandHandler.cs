using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : ResponseHandler, IRequestHandler<ResetPasswordCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;

        public ResetPasswordCommandHandler(
            IStringLocalizer<SharedResources> stringLocalizer,
            UserManager<User> userManager) : base(stringLocalizer)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.EmailIsNotExist]);
            }

            if (!user.IsActive)
            {
                return Unauthorized<string>(_stringLocalizer[SharedResourcesKeys.UserIsNotActive]);
            }

            // Reset password with token
            var result = await _userManager.ResetPasswordAsync(user, request.ResetCode, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            return Success<string>(_stringLocalizer[SharedResourcesKeys.Updated]);
        }
    }
}
