using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRoute
{
    public class UpdateRouteCommand : IRequest<Response<RouteDto>>
    {
        public int Id { get; set; }
        public int? OriginStationId { get; set; }
        public int? DestinationStationId { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public decimal? DistanceKm { get; set; }
        public bool? IsActive { get; set; }
        public string? MaintenanceNote { get; set; }
    }
}

