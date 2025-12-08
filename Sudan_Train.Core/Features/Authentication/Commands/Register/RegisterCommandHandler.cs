using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : ResponseHandler, IRequestHandler<RegisterCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public RegisterCommandHandler(UserManager<User> userManager, IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _userManager = userManager;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<Response<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if username already exists
            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.UserNameIsExist]);
            }

            // Check if email already exists
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.EmailIsExist]);
            }

            // Create new user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToAddUser]);
            }

            return Created<string>(_stringLocalizer[SharedResourcesKeys.UserRegisteredSuccessfully], new { user.UserName, user.Email });
        }
    }
}

