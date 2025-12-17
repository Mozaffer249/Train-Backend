using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Service.Abstracts;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Sudan_Train.Core.Features.Authentication.Commands.TrustDevice
{
    public class TrustDeviceCommandHandler : ResponseHandler, IRequestHandler<TrustDeviceCommand, Response<string>>
    {
        private readonly ISessionManagementService _sessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public TrustDeviceCommandHandler(
            ISessionManagementService sessionService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer) : base(authLocalizer)
        {
            _sessionService = sessionService;
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<string>> Handle(TrustDeviceCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            var fingerprint = request.DeviceFingerprint ?? Guid.NewGuid().ToString();

            await _sessionService.AddTrustedDeviceAsync(userId, request.DeviceId, request.DeviceName, fingerprint);

            return Success<string>("Device trusted successfully");
        }
    }
}
