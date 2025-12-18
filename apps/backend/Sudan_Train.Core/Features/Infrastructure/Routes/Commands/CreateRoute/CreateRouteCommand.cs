using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.CreateRoute
{
    public class CreateRouteCommand : IRequest<Response<RouteDto>>
    {
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public int OriginStationId { get; set; }
        public int DestinationStationId { get; set; }
        public decimal? DistanceKm { get; set; }
    }
}

