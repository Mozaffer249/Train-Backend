using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRoute
{
    public class UpdateRouteCommandHandler : ResponseHandler, IRequestHandler<UpdateRouteCommand, Response<RouteDto>>
    {
        private readonly IRouteService _routeService;

        public UpdateRouteCommandHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<RouteDto>> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var routeDto = await _routeService.UpdateRouteAsync(
                    request.Id,
                    request.OriginStationId,
                    request.DestinationStationId,
                    request.NameEn,
                    request.NameAr,
                    request.DistanceKm,
                    request.IsActive,
                    request.MaintenanceNote);
                return Success("Route updated successfully", routeDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<RouteDto>("Route not found");
            }
        }
    }
}

