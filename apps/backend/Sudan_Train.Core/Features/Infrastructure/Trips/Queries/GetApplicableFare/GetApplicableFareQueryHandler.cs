using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetApplicableFare
{
    public class GetApplicableFareQueryHandler : ResponseHandler, IRequestHandler<GetApplicableFareQuery, Response<FareDto>>
    {
        private readonly IFareService _fareService;
        private readonly ITripRepository _tripRepository;

        public GetApplicableFareQueryHandler(
            IFareService fareService,
            ITripRepository tripRepository,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _fareService = fareService;
            _tripRepository = tripRepository;
        }

        public async Task<Response<FareDto>> Handle(GetApplicableFareQuery request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(request.TripId);
            if (trip == null)
                return NotFound<FareDto>("Trip not found.");

            var fare = await _fareService.GetApplicableFareAsync(
                routeId: trip.RouteId,
                originStationId: request.BoardingStationId,
                destinationStationId: request.AlightingStationId,
                tripId: request.TripId,
                coachClass: request.CoachClass);

            if (fare == null)
                return NotFound<FareDto>("No applicable fare found for this trip + segment + class.");

            return Success(null, fare);
        }
    }
}
