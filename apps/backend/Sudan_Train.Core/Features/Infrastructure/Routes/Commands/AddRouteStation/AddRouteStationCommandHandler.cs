using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.AddRouteStation
{
    public class AddRouteStationCommandHandler : ResponseHandler, IRequestHandler<AddRouteStationCommand, Response<RouteStationDto>>
    {
        private readonly IRouteService _routeService;

        public AddRouteStationCommandHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<RouteStationDto>> Handle(AddRouteStationCommand request, CancellationToken cancellationToken)
        {
            var routeStationDto = await _routeService.AddRouteStationAsync(
                request.RouteId,
                request.StationId,
                request.StopOrder,
                request.ArrivalMinutesFromOrigin,
                request.DepartureMinutesFromOrigin);
            return Success("Route station added successfully", routeStationDto);
        }
    }
}

