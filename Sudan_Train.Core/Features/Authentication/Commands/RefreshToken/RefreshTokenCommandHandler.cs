using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Data.Results;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : ResponseHandler, IRequestHandler<RefreshTokenCommand, Response<JwtAuthResult>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public RefreshTokenCommandHandler(
            IStringLocalizer<SharedResources> sharedLocalizer,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            IAuthenticationService authenticationService,
            UserManager<User> userManager) : base(sharedLocalizer)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
            _sharedLocalizer = sharedLocalizer;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<JwtAuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate refresh token
            var jwtToken = await _authenticationService.ReadJWTToken(request.AccessToken);

            if (jwtToken == null)
            {
                return BadRequest<JwtAuthResult>(_sharedLocalizer[SharedResourcesKeys.UnAuthorized]);
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest<JwtAuthResult>(_sharedLocalizer[SharedResourcesKeys.UnAuthorized]);
            }

            var user = await _userManager.FindByIdAsync(userIdClaim);

            if (user == null)
            {
                return NotFound<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            if (!user.IsActive)
            {
                return Unauthorized<JwtAuthResult>(_authLocalizer[AuthenticationResourcesKeys.UserIsNotActive]);
            }

            // Validate and refresh token
            var result = await _authenticationService.GetRefreshToken(user, jwtToken, request.RefreshToken);

            if (result == null)
            {
                return Unauthorized<JwtAuthResult>(_sharedLocalizer[SharedResourcesKeys.UnAuthorized]);
            }

            return Success(entity: result);
        }
    }
}
