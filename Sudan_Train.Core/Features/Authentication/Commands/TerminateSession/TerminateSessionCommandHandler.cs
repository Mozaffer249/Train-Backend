using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Service.Abstracts;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Commands.TerminateSession
{
    public class TerminateSessionCommandHandler : ResponseHandler, IRequestHandler<TerminateSessionCommand, Response<string>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly ISessionManagementService _sessionService;

        public TerminateSessionCommandHandler(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            ISessionManagementService sessionService) : base(authLocalizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
            _sessionService = sessionService;
        }

        public async Task<Response<string>> Handle(TerminateSessionCommand request, CancellationToken cancellationToken)
        {
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Terminate the session (the service will validate ownership)
            var result = await _sessionService.TerminateSessionAsync((int)request.SessionId, userId);

            if (!result)
            {
                return NotFound<string>(_authLocalizer[AuthenticationResourcesKeys.SessionNotFound]);
            }

            return Success<string>(_authLocalizer[AuthenticationResourcesKeys.SessionTerminated]);
        }
    }
}

