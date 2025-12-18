using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.AddRouteStation
{
    public class AddRouteStationCommand : IRequest<Response<RouteStationDto>>
    {
        public int RouteId { get; set; }
        public int StationId { get; set; }
        public int StopOrder { get; set; }
        public int ArrivalMinutesFromOrigin { get; set; }
        public int DepartureMinutesFromOrigin { get; set; }
    }
}

