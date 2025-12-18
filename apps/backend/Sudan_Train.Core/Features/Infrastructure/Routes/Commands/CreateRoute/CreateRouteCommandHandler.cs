using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.CreateRoute
{
    public class CreateRouteCommandHandler : ResponseHandler, IRequestHandler<CreateRouteCommand, Response<RouteDto>>
    {
        private readonly IRouteService _routeService;

        public CreateRouteCommandHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<RouteDto>> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            var routeDto = await _routeService.CreateRouteAsync(
                request.OriginStationId,
                request.DestinationStationId,
                request.NameEn,
                request.NameAr,
                request.DistanceKm);
            return Success("Route created successfully", routeDto);
        }
    }
}

