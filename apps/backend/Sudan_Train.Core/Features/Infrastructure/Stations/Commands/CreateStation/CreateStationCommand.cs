using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.CreateStation
{
    public class CreateStationCommand : IRequest<Response<StationDto>>
    {
        public string Code { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int CityId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? AddressEn { get; set; }
        public string? AddressAr { get; set; }
    }
}

