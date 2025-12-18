using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CreateTrip
{
    public class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
    {
        private readonly ITrainRepository _trainRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly ITripService _tripService;

        public CreateTripCommandValidator(
            ITrainRepository trainRepository,
            IRouteRepository routeRepository,
            ITripService tripService)
        {
            _trainRepository = trainRepository;
            _routeRepository = routeRepository;
            _tripService = tripService;

            RuleFor(x => x.TrainId)
                .GreaterThan(0).WithMessage("Train ID is required")
                .MustAsync(TrainExists).WithMessage("Train not found");

            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("Route ID is required")
                .MustAsync(RouteExists).WithMessage("Route not found");

            RuleFor(x => x.DepartureTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Departure time must be in the future");

            RuleFor(x => x.ArrivalTime)
                .GreaterThan(x => x.DepartureTime).WithMessage("Arrival time must be after departure time");

            RuleFor(x => x)
                .MustAsync(NotHaveOverlappingTrips).WithMessage("Train already has a trip scheduled during this time");
        }

        private async Task<bool> TrainExists(int trainId, CancellationToken cancellationToken)
        {
            return await _trainRepository.GetTableNoTracking().AnyAsync(t => t.Id == trainId, cancellationToken);
        }

        private async Task<bool> RouteExists(int routeId, CancellationToken cancellationToken)
        {
            return await _routeRepository.GetTableNoTracking().AnyAsync(r => r.Id == routeId, cancellationToken);
        }

        private async Task<bool> NotHaveOverlappingTrips(CreateTripCommand command, CancellationToken cancellationToken)
        {
            return !await _tripService.HasOverlappingTripsAsync(command.TrainId, command.DepartureTime, command.ArrivalTime);
        }
    }
}

