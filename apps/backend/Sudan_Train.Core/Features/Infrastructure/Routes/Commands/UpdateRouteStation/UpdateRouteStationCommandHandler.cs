using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRouteStation
{
    public class UpdateRouteStationCommandHandler : ResponseHandler, IRequestHandler<UpdateRouteStationCommand, Response<RouteStationDto>>
    {
        private readonly IRouteService _routeService;

        public UpdateRouteStationCommandHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<RouteStationDto>> Handle(UpdateRouteStationCommand request, CancellationToken cancellationToken)
        {
            var routeStationDto = await _routeService.UpdateRouteStationAsync(
                request.RouteId,
                request.StationId,
                request.StopOrder,
                request.ArrivalMinutesFromOrigin,
                request.DepartureMinutesFromOrigin);

            if (routeStationDto == null)
                return NotFound<RouteStationDto>("Route station not found");

            return Success("Route station updated successfully", routeStationDto);
        }
    }
}
