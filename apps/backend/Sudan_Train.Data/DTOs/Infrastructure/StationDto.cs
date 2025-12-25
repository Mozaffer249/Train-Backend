namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class StationDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int CityId { get; set; }
        public string CityName { get; set; } = default!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? AddressEn { get; set; }
        public string? AddressAr { get; set; }
        public string? GooglePlaceId { get; set; }
        public string? FormattedAddress { get; set; }
        public double? ServiceRadiusKm { get; set; }
        public string? StationType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

