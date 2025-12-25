using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.ValidateLocation
{
    public class ValidateStationLocationQuery : IRequest<Response<StationValidationDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CityId { get; set; }
        public int? ExcludeStationId { get; set; } // For update scenarios
    }
}
