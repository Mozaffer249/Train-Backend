using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetTripById
{
    public class GetTripByIdQueryHandler : ResponseHandler, IRequestHandler<GetTripByIdQuery, Response<TripDto>>
    {
        private readonly ITripService _tripService;

        public GetTripByIdQueryHandler(
            ITripService tripService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
        }

        public async Task<Response<TripDto>> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
        {
            var trip = await _tripService.GetTripByIdAsync(request.Id);
            if (trip == null)
                return NotFound<TripDto>("Trip not found");

            return Success(null, trip);
        }
    }
}

