using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRoute
{
    public class UpdateRouteCommandValidator : AbstractValidator<UpdateRouteCommand>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IStationRepository _stationRepository;
        private readonly ITripRepository _tripRepository;

        public UpdateRouteCommandValidator(
            IRouteRepository routeRepository,
            IStationRepository stationRepository,
            ITripRepository tripRepository)
        {
            _routeRepository = routeRepository;
            _stationRepository = stationRepository;
            _tripRepository = tripRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Route ID is required")
                .MustAsync(RouteExists).WithMessage("Route not found");

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).When(x => x.DistanceKm.HasValue)
                .WithMessage("Distance must be greater than 0");

            // Validate origin station exists if provided
            RuleFor(x => x.OriginStationId)
                .MustAsync(StationExists)
                .When(x => x.OriginStationId.HasValue)
                .WithMessage("Origin station not found");

            // Validate destination station exists if provided
            RuleFor(x => x.DestinationStationId)
                .MustAsync(StationExists)
                .When(x => x.DestinationStationId.HasValue)
                .WithMessage("Destination station not found");

            // Validate origin != destination
            RuleFor(x => x)
                .Must(x => !x.OriginStationId.HasValue || !x.DestinationStationId.HasValue ||
                           x.OriginStationId != x.DestinationStationId)
                .WithMessage("Origin and destination must be different stations");

            // Block update if route has trips and origin/dest changing
            RuleFor(x => x)
                .MustAsync(CanUpdateStations)
                .When(x => x.OriginStationId.HasValue || x.DestinationStationId.HasValue)
                .WithMessage("Cannot update origin/destination: route has existing trips");

            // Validate maintenance note length
            RuleFor(x => x.MaintenanceNote)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.MaintenanceNote))
                .WithMessage("Maintenance note cannot exceed 500 characters");
        }

        private async Task<bool> RouteExists(int id, CancellationToken cancellationToken)
        {
            return await _routeRepository.GetTableNoTracking().AnyAsync(r => r.Id == id, cancellationToken);
        }

        private async Task<bool> StationExists(int? stationId, CancellationToken cancellationToken)
        {
            if (!stationId.HasValue) return true;
            return await _stationRepository.GetTableNoTracking()
                .AnyAsync(s => s.Id == stationId.Value, cancellationToken);
        }

        private async Task<bool> CanUpdateStations(UpdateRouteCommand command, CancellationToken cancellationToken)
        {
            // If not changing stations, allow
            if (!command.OriginStationId.HasValue && !command.DestinationStationId.HasValue)
                return true;

            // Check if route has trips
            var hasTrips = await _tripRepository.GetTableNoTracking()
                .AnyAsync(t => t.RouteId == command.Id, cancellationToken);

            return !hasTrips;
        }
    }
}

