using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetRouteById
{
    public class GetRouteByIdQueryHandler : ResponseHandler, IRequestHandler<GetRouteByIdQuery, Response<RouteDto>>
    {
        private readonly IRouteService _routeService;

        public GetRouteByIdQueryHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<RouteDto>> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
        {
            var route = await _routeService.GetRouteByIdAsync(request.Id);
            if (route == null)
                return NotFound<RouteDto>("Route not found");

            return Success(null, route);
        }
    }
}

