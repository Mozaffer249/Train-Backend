using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetAllRoutes
{
    public class GetAllRoutesQueryHandler : ResponseHandler, IRequestHandler<GetAllRoutesQuery, Response<List<RouteDto>>>
    {
        private readonly IRouteService _routeService;

        public GetAllRoutesQueryHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<List<RouteDto>>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
        {
            var routes = await _routeService.GetAllRoutesAsync(request.OriginStationId, request.DestinationStationId);
            return Success(null, routes);
        }
    }
}

