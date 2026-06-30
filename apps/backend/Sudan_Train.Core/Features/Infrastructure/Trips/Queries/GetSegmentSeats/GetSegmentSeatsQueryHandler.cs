using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetSegmentSeats
{
    public class GetSegmentSeatsQueryHandler : ResponseHandler, IRequestHandler<GetSegmentSeatsQuery, Response<SegmentSeatsDto>>
    {
        private readonly ITripService _tripService;
        private readonly IHttpContextAccessor _http;

        public GetSegmentSeatsQueryHandler(
            ITripService tripService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
            _http = http;
        }

        public async Task<Response<SegmentSeatsDto>> Handle(GetSegmentSeatsQuery request, CancellationToken cancellationToken)
        {
            int? currentUserId = null;
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            if (int.TryParse(userIdClaim, out var uid) && uid > 0)
                currentUserId = uid;

            var seats = await _tripService.GetSegmentSeatsAsync(
                request.TripId,
                request.BoardingStationId,
                request.AlightingStationId,
                currentUserId);

            if (seats == null)
                return NotFound<SegmentSeatsDto>("Trip not found or stations are not on its route in the correct order.");

            return Success(null, seats);
        }
    }
}
