namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class CityDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public string? FormattedAddress { get; set; }

        // Boundary data
        public string? BoundaryPolygon { get; set; }
        public double? BoundingBoxNorth { get; set; }
        public double? BoundingBoxSouth { get; set; }
        public double? BoundingBoxEast { get; set; }
        public double? BoundingBoxWest { get; set; }

        public int StationsCount { get; set; }
    }
}

