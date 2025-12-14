using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Authentication.Commands.Login;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Data.Results;
using Sudan_Train.Service.Abstracts;

namespace Trains.Core.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : ResponseHandler, IRequestHandler<LoginCommand, Response<JwtAuthResult>>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public LoginCommandHandler(
            IStringLocalizer<AuthenticationResources> authLocalizer,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthenticationService authenticationService) : base(authLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<JwtAuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists (validation ensures UserName is not null)
            var user = await _userManager.FindByNameAsync(request.UserName!);
            if (user == null)
            {
                return NotFound<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserIsNotActive]);
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.EmailNotConfirmed]);
            }

            // Try to sign in with lockout enabled (validation ensures Password is not null)
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, true);

            // Check if account is locked out
            if (signInResult.IsLockedOut)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.AccountLockedOut]);
            }

            // Check if 2FA is required
            if (signInResult.RequiresTwoFactor || user.TwoFactorEnabled)
            {
                return BadRequest<JwtAuthResult>("Two-factor authentication is required. Please use LoginWithTwoFactor endpoint.");
            }

            if (!signInResult.Succeeded)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.PasswordNotCorrect]);
            }

            // Generate JWT token
            var result = await _authenticationService.GetJWTToken(user);

            return Success(entity: result);
        }
    }
}

