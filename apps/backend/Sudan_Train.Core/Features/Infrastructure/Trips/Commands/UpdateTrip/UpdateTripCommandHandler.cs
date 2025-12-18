using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.UpdateTrip
{
    public class UpdateTripCommandHandler : ResponseHandler, IRequestHandler<UpdateTripCommand, Response<TripDto>>
    {
        private readonly ITripService _tripService;

        public UpdateTripCommandHandler(
            ITripService tripService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
        }

        public async Task<Response<TripDto>> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tripDto = await _tripService.UpdateTripAsync(
                    request.Id,
                    request.DepartureTime,
                    request.ArrivalTime,
                    request.Status);
                return Success("Trip updated successfully", tripDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<TripDto>("Trip not found");
            }
        }
    }
}

