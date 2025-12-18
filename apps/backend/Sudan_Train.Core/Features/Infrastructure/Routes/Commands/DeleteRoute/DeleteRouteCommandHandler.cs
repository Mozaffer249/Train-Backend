using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.DeleteRoute
{
    public class DeleteRouteCommandHandler : ResponseHandler, IRequestHandler<DeleteRouteCommand, Response<string>>
    {
        private readonly IRouteService _routeService;

        public DeleteRouteCommandHandler(
            IRouteService routeService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _routeService = routeService;
        }

        public async Task<Response<string>> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
        {
            var hasTrips = await _routeService.RouteHasTripsAsync(request.Id);
            if (hasTrips)
                return BadRequest<string>("Cannot delete route because it has trips");

            var deleted = await _routeService.DeleteRouteAsync(request.Id);
            if (!deleted)
                return NotFound<string>("Route not found");

            return Success<string>("Route deleted successfully");
        }
    }
}

