using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.AddRouteStation
{
    public class AddRouteStationCommandValidator : AbstractValidator<AddRouteStationCommand>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IRouteStationRepository _routeStationRepository;

        public AddRouteStationCommandValidator(
            IRouteRepository routeRepository,
            IStationRepository stationRepository,
            IRouteStationRepository routeStationRepository)
        {
            _routeRepository = routeRepository;
            _stationRepository = stationRepository;
            _routeStationRepository = routeStationRepository;

            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("Route ID is required")
                .MustAsync(RouteExists).WithMessage("Route not found");

            RuleFor(x => x.StationId)
                .GreaterThan(0).WithMessage("Station ID is required")
                .MustAsync(StationExists).WithMessage("Station not found")
                .MustAsync(NotOriginOrDestination).WithMessage("Station cannot be the origin or destination of the route");

            RuleFor(x => x.StopOrder)
                .GreaterThan(0).WithMessage("Stop order must be greater than 0")
                .MustAsync(BeUniqueStopOrder).WithMessage("Stop order already exists for this route");

            RuleFor(x => x.DepartureMinutesFromOrigin)
                .GreaterThan(x => x.ArrivalMinutesFromOrigin)
                .WithMessage("Departure time must be after arrival time");
        }

        private async Task<bool> RouteExists(int routeId, CancellationToken cancellationToken)
        {
            return await _routeRepository.GetTableNoTracking().AnyAsync(r => r.Id == routeId, cancellationToken);
        }

        private async Task<bool> StationExists(int stationId, CancellationToken cancellationToken)
        {
            return await _stationRepository.GetTableNoTracking().AnyAsync(s => s.Id == stationId, cancellationToken);
        }

        private async Task<bool> NotOriginOrDestination(AddRouteStationCommand command, int stationId, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetByIdAsync(command.RouteId);
            if (route == null) return true;
            return stationId != route.OriginStationId && stationId != route.DestinationStationId;
        }

        private async Task<bool> BeUniqueStopOrder(AddRouteStationCommand command, int stopOrder, CancellationToken cancellationToken)
        {
            return !await _routeStationRepository.GetTableNoTracking()
                .AnyAsync(rs => rs.RouteId == command.RouteId && rs.StopOrder == stopOrder, cancellationToken);
        }
    }
}

