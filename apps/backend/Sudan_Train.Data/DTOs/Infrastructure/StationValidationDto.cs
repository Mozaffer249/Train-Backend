namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class StationValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public StationDto? ExistingStation { get; set; }
        public StationLocationSuggestion? SuggestedData { get; set; }
        public double? DistanceKm { get; set; }
    }

    public class StationLocationSuggestion
    {
        public string NameEn { get; set; } = string.Empty;
        public string FormattedAddress { get; set; } = string.Empty;
        public string? GooglePlaceId { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
    }

    public class ValidateStationLocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CityId { get; set; }
    }
}
