using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.CreateRoute
{
    public class CreateRouteCommandValidator : AbstractValidator<CreateRouteCommand>
    {
        private readonly IStationRepository _stationRepository;
        private readonly IRouteRepository _routeRepository;

        public CreateRouteCommandValidator(IStationRepository stationRepository, IRouteRepository routeRepository)
        {
            _stationRepository = stationRepository;
            _routeRepository = routeRepository;

            RuleFor(x => x.OriginStationId)
                .GreaterThan(0).WithMessage("Origin station ID is required")
                .MustAsync(StationExists).WithMessage("Origin station not found")
                .NotEqual(x => x.DestinationStationId).WithMessage("Origin and destination stations must be different");

            RuleFor(x => x.DestinationStationId)
                .GreaterThan(0).WithMessage("Destination station ID is required")
                .MustAsync(StationExists).WithMessage("Destination station not found");

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).When(x => x.DistanceKm.HasValue)
                .WithMessage("Distance must be greater than 0");

            RuleFor(x => x)
                .MustAsync(BeUniqueRoute).WithMessage("A route with this origin and destination already exists");
        }

        private async Task<bool> StationExists(int stationId, CancellationToken cancellationToken)
        {
            return await _stationRepository.GetTableNoTracking().AnyAsync(s => s.Id == stationId, cancellationToken);
        }

        private async Task<bool> BeUniqueRoute(CreateRouteCommand command, CancellationToken cancellationToken)
        {
            return !await _routeRepository.GetTableNoTracking()
                .AnyAsync(r => (r.OriginStationId == command.OriginStationId && r.DestinationStationId == command.DestinationStationId) ||
                              (r.OriginStationId == command.DestinationStationId && r.DestinationStationId == command.OriginStationId),
                       cancellationToken);
        }
    }
}

