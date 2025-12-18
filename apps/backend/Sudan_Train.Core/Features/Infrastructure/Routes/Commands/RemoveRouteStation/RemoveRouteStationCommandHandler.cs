using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.RemoveRouteStation
{
    public class RemoveRouteStationCommandHandler : ResponseHandler, IRequestHandler<RemoveRouteStationCommand, Response<string>>
    {
        private readonly IRouteService _routeService;

        public RemoveRouteStationCommandHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<string>> Handle(RemoveRouteStationCommand request, CancellationToken cancellationToken)
        {
            var removed = await _routeService.RemoveRouteStationAsync(request.RouteId, request.StationId);
            if (!removed)
                return NotFound<string>("Route station not found");

            return Success<string>("Route station removed successfully");
        }
    }
}

