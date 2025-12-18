using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.RemoveRouteStation
{
    public class RemoveRouteStationCommand : IRequest<Response<string>>
    {
        public int RouteId { get; set; }
        public int StationId { get; set; }
    }
}

