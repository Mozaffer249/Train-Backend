using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Users.Commands.SetUserActive
{
    public class SetUserActiveCommandHandler : ResponseHandler, IRequestHandler<SetUserActiveCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;

        public SetUserActiveCommandHandler(
            UserManager<User> userManager,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(SetUserActiveCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return NotFound<string>("User not found.");

            if (user.IsActive == request.IsActive)
                return Success(request.IsActive ? "User already active" : "User already inactive", string.Empty);

            user.IsActive = request.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest<string>(string.Join("; ", result.Errors.Select(e => e.Description)));

            return Success(request.IsActive ? "User enabled" : "User disabled", string.Empty);
        }
    }
}
