using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetSegmentSeats
{
    public class GetSegmentSeatsQueryHandler : ResponseHandler, IRequestHandler<GetSegmentSeatsQuery, Response<SegmentSeatsDto>>
    {
        private readonly ITripService _tripService;

        public GetSegmentSeatsQueryHandler(
            ITripService tripService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
        }

        public async Task<Response<SegmentSeatsDto>> Handle(GetSegmentSeatsQuery request, CancellationToken cancellationToken)
        {
            var seats = await _tripService.GetSegmentSeatsAsync(
                request.TripId,
                request.BoardingStationId,
                request.AlightingStationId);

            if (seats == null)
                return NotFound<SegmentSeatsDto>("Trip not found or stations are not on its route in the correct order.");

            return Success(null, seats);
        }
    }
}
