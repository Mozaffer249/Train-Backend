using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Data.Results;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : ResponseHandler, IRequestHandler<LoginCommand, Response<JwtAuthResult>>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(
            IStringLocalizer<SharedResources> stringLocalizer,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthenticationService authenticationService) : base(stringLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
        }

        public async Task<Response<JwtAuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return NotFound<JwtAuthResult>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Unauthorized<JwtAuthResult>(_stringLocalizer[SharedResourcesKeys.UserIsNotActive]);
            }

            // Try to sign in
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                return Unauthorized<JwtAuthResult>(_stringLocalizer[SharedResourcesKeys.PasswordNotCorrect]);
            }

            // Generate JWT token
            var result = await _authenticationService.GetJWTToken(user);

            return Success(result);
        }
    }
}

