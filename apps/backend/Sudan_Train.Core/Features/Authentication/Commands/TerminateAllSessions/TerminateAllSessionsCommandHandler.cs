using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Service.Abstracts;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Commands.TerminateAllSessions
{
    public class TerminateAllSessionsCommandHandler : ResponseHandler, IRequestHandler<TerminateAllSessionsCommand, Response<string>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly ISessionManagementService _sessionService;

        public TerminateAllSessionsCommandHandler(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            ISessionManagementService sessionService) : base(authLocalizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
            _sessionService = sessionService;
        }

        public async Task<Response<string>> Handle(TerminateAllSessionsCommand request, CancellationToken cancellationToken)
        {
            // Session management temporarily disabled — LoginSession table dropped (DropAdvancedSecurityTables migration).
            await Task.CompletedTask;
            return BadRequest<string>("Session management is temporarily disabled.");

            /* Original implementation preserved for restoration:
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            string? currentAccessToken = null;
            if (request.ExceptCurrent)
            {
                // Get current access token from the authorization header
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    currentAccessToken = authHeader.Substring(7);
                }
            }

            await _sessionService.TerminateAllSessionsExceptCurrentAsync(userId, currentAccessToken ?? string.Empty);

            return Success<string>(_authLocalizer[AuthenticationResourcesKeys.AllSessionsTerminated]);
            */
        }
    }
}

