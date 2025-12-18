using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CreateTrip
{
    public class CreateTripCommandHandler : ResponseHandler, IRequestHandler<CreateTripCommand, Response<TripDto>>
    {
        private readonly ITripService _tripService;

        public CreateTripCommandHandler(
            ITripService tripService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
        }

        public async Task<Response<TripDto>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var tripDto = await _tripService.CreateTripAsync(
                request.TrainId,
                request.RouteId,
                request.DepartureTime,
                request.ArrivalTime);
            return Success("Trip created successfully", tripDto);
        }
    }
}

