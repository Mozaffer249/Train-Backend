using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetAllStations
{
    public class GetAllStationsQuery : IRequest<Response<List<StationDto>>>
    {
        public int? CityId { get; set; }
        public string? SearchTerm { get; set; }
    }
}

