using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : ResponseHandler, IRequestHandler<ConfirmEmailCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;

        public ConfirmEmailCommandHandler(
            IStringLocalizer<SharedResources> stringLocalizer,
            UserManager<User> userManager) : base(stringLocalizer)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            // Find user
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);
            }

            // Confirm email
            var result = await _userManager.ConfirmEmailAsync(user, request.Code);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            return Success<string>(_stringLocalizer[SharedResourcesKeys.Success]);
        }
    }
}
