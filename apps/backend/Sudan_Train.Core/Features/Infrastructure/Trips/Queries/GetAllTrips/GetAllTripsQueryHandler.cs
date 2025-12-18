using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetAllTrips
{
    public class GetAllTripsQueryHandler : ResponseHandler, IRequestHandler<GetAllTripsQuery, Response<List<TripDto>>>
    {
        private readonly ITripService _tripService;

        public GetAllTripsQueryHandler(
            ITripService tripService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
        }

        public async Task<Response<List<TripDto>>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
        {
            var trips = await _tripService.GetAllTripsAsync(request.Date, request.RouteId, request.Status);
            return Success(null, trips);
        }
    }
}

