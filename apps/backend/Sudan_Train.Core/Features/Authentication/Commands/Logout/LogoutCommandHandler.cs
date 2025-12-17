using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Service.Abstracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Commands.Logout
{
    public class LogoutCommandHandler : ResponseHandler, IRequestHandler<LogoutCommand, Response<string>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public LogoutCommandHandler(
            IAuthenticationService authenticationService,
            IStringLocalizer<AuthenticationResources> authLocalizer) : base(authLocalizer)
        {
            _authenticationService = authenticationService;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Extract user ID from token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(request.AccessToken);
            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "uid");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Revoke token(s)
            var result = await _authenticationService.RevokeTokenAsync(
                request.AccessToken,
                request.RefreshToken,
                userId,
                request.LogoutAllDevices
            );

            if (!result)
            {
                return BadRequest<string>("Failed to logout");
            }

            return Success<string>("Logged out successfully");
        }
    }
}

