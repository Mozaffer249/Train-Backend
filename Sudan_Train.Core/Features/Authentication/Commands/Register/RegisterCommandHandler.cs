using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : ResponseHandler, IRequestHandler<RegisterCommand, Response<object>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public RegisterCommandHandler(UserManager<User> userManager, IStringLocalizer<AuthenticationResources> authLocalizer) : base(authLocalizer)
        {
            _userManager = userManager;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists (validation ensures Email is not null)
            var existingEmail = await _userManager.FindByEmailAsync(request.Email!);
            if (existingEmail != null)
            {
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.EmailIsExist]);
            }

            // Create new user (validation ensures all required fields are not null)
            var user = new User
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                UserName = request.Email!.Split('@')[0],
                Email = request.Email!,
                PhoneNumber = request.PhoneNumber,
                IsActive = false
            };
            var result = await _userManager.CreateAsync(user, request.Password!);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.FailedToAddUser]);
            }

            return Created<object>(
                _authLocalizer[AuthenticationResourcesKeys.UserRegisteredSuccessfully],
                entity: new { user.Id, user.UserName, user.Email, user.FirstName, user.LastName });
        }
    }
}