using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.MarkTripArrived
{
    public class MarkTripArrivedCommandHandler
        : ResponseHandler, IRequestHandler<MarkTripArrivedCommand, Response<string>>
    {
        private readonly ITripService _tripService;
        private readonly IStaffAuthorizationService _staffAuth;
        private readonly IHttpContextAccessor _http;

        public MarkTripArrivedCommandHandler(
            ITripService tripService,
            IStaffAuthorizationService staffAuth,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
            _staffAuth = staffAuth;
            _http = http;
        }

        public async Task<Response<string>> Handle(MarkTripArrivedCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);
            var roles = _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? new List<string>();

            if (userId > 0 && !await _staffAuth.CanOperateTripAsync(userId, roles, request.Id))
                return Unauthorized<string>("Not assigned to a station on this trip.");

            var ok = await _tripService.MarkArrivedAsync(request.Id);
            if (!ok)
                return BadRequest<string>("Trip cannot be marked arrived.");

            return Success<string>("Trip marked arrived.");
        }
    }
}
