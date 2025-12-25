using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.UpdateCity
{
    public class UpdateCityCommand : IRequest<Response<CityDto>>
    {
        public int Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public string? FormattedAddress { get; set; }

        // Boundary fields
        public string? BoundaryPolygon { get; set; }
        public double? BoundingBoxNorth { get; set; }
        public double? BoundingBoxSouth { get; set; }
        public double? BoundingBoxEast { get; set; }
        public double? BoundingBoxWest { get; set; }
    }
}

