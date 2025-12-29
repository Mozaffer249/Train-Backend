using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRouteStation
{
    public class UpdateRouteStationCommandValidator : AbstractValidator<UpdateRouteStationCommand>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IRouteStationRepository _routeStationRepository;

        public UpdateRouteStationCommandValidator(
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
                .MustAsync(StationExists).WithMessage("Station not found");

            RuleFor(x => x.StopOrder)
                .GreaterThan(0).When(x => x.StopOrder.HasValue)
                .WithMessage("Stop order must be greater than 0")
                .MustAsync(BeUniqueStopOrderForUpdate).When(x => x.StopOrder.HasValue)
                .WithMessage("Stop order already exists for this route");

            RuleFor(x => x.DepartureMinutesFromOrigin)
                .GreaterThan(x => x.ArrivalMinutesFromOrigin)
                .When(x => x.ArrivalMinutesFromOrigin.HasValue && x.DepartureMinutesFromOrigin.HasValue)
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

        private async Task<bool> BeUniqueStopOrderForUpdate(UpdateRouteStationCommand command, int? stopOrder, CancellationToken cancellationToken)
        {
            if (!stopOrder.HasValue) return true;

            return !await _routeStationRepository.GetTableNoTracking()
                .AnyAsync(rs => rs.RouteId == command.RouteId &&
                              rs.StopOrder == stopOrder.Value &&
                              rs.StationId != command.StationId, cancellationToken);
        }
    }
}
