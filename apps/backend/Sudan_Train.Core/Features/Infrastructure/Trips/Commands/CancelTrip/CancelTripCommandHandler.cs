using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CancelTrip
{
    public class CancelTripCommandHandler : ResponseHandler, IRequestHandler<CancelTripCommand, Response<string>>
    {
        private readonly ITripService _tripService;
        private readonly IHttpContextAccessor _http;

        public CancelTripCommandHandler(
            ITripService tripService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
            _http = http;
        }

        public async Task<Response<string>> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);

            var cancelled = await _tripService.CancelTripWithCascadeAsync(request.Id, userId, request.Reason);
            if (!cancelled)
                return BadRequest<string>("Trip not found or cannot be cancelled");

            return Success<string>("Trip cancelled successfully");
        }
    }
}
