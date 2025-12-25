namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class CityValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public CityDto? ExistingCity { get; set; }
        public CityLocationSuggestion? SuggestedData { get; set; }
        public double? DistanceKm { get; set; }
    }

    public class CityLocationSuggestion
    {
        public string NameEn { get; set; } = string.Empty;
        public string FormattedAddress { get; set; } = string.Empty;
        public string? GooglePlaceId { get; set; }
    }

    public class ValidateCityLocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
