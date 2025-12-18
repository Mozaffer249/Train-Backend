using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.UpdateStation
{
    public class UpdateStationCommand : IRequest<Response<StationDto>>
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int CityId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AddressEn { get; set; }
        public string? AddressAr { get; set; }
    }
}

