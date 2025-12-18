using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetAllRoutes
{
    public class GetAllRoutesQuery : IRequest<Response<List<RouteDto>>>
    {
        public int? OriginStationId { get; set; }
        public int? DestinationStationId { get; set; }
    }
}

