using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Data.Results;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Authentication.Commands.LoginWithTwoFactor
{
    public class LoginWithTwoFactorCommandHandler : ResponseHandler, IRequestHandler<LoginWithTwoFactorCommand, Response<JwtAuthResult>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITwoFactorAuthenticationService _twoFactorService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public LoginWithTwoFactorCommandHandler(
            UserManager<User> userManager,
            ITwoFactorAuthenticationService twoFactorService,
            IAuthenticationService authenticationService,
            IStringLocalizer<AuthenticationResources> authLocalizer) : base(authLocalizer)
        {
            _userManager = userManager;
            _twoFactorService = twoFactorService;
            _authenticationService = authenticationService;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<JwtAuthResult>> Handle(LoginWithTwoFactorCommand request, CancellationToken cancellationToken)
        {
            // Two-factor login temporarily disabled — TwoFactorRecoveryCode table dropped (DropAdvancedSecurityTables migration).
            await Task.CompletedTask;
            return BadRequest<JwtAuthResult>("Two-factor authentication is temporarily disabled.");

            /* Original implementation preserved for restoration:
            // Find user
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return NotFound<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserIsNotActive]);
            }

            // Check if 2FA is enabled
            if (!user.TwoFactorEnabled)
            {
                return BadRequest<JwtAuthResult>("Two-factor authentication is not enabled for this account");
            }

            bool isValidCode;

            if (request.UseRecoveryCode)
            {
                // Validate recovery code
                isValidCode = await _twoFactorService.UseRecoveryCodeAsync(user.Id, request.Code);
            }
            else
            {
                // Validate TOTP code
                isValidCode = await _twoFactorService.ValidateTwoFactorCodeAsync(user.Id, request.Code);
            }

            if (!isValidCode)
            {
                return Unauthorized<JwtAuthResult>("Invalid two-factor authentication code");
            }

            // Generate JWT token
            var result = await _authenticationService.GetJWTToken(user);

            return Success<JwtAuthResult>(entity: result);
            */
        }
    }
}
