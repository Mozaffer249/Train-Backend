using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sudan_Train.Core.Services.Google.Models
{
    public class GooglePlacesResponse
    {
        [JsonPropertyName("results")]
        public List<GooglePlaceResult> Results { get; set; } = new List<GooglePlaceResult>();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("next_page_token")]
        public string? NextPageToken { get; set; }
    }

    public class GooglePlaceResult
    {
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("vicinity")]
        public string? Vicinity { get; set; }

        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("geometry")]
        public GoogleGeometry Geometry { get; set; } = new GoogleGeometry();

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = new List<string>();

        [JsonPropertyName("business_status")]
        public string? BusinessStatus { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("user_ratings_total")]
        public int? UserRatingsTotal { get; set; }
    }

    public class GooglePlaceDetailsResponse
    {
        [JsonPropertyName("result")]
        public GooglePlaceDetails? Result { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }
    }

    public class GooglePlaceDetails : GooglePlaceResult
    {
        [JsonPropertyName("formatted_phone_number")]
        public string? FormattedPhoneNumber { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("opening_hours")]
        public GoogleOpeningHours? OpeningHours { get; set; }

        [JsonPropertyName("photos")]
        public List<GooglePhoto>? Photos { get; set; }

        [JsonPropertyName("reviews")]
        public List<GoogleReview>? Reviews { get; set; }
    }

    public class GoogleOpeningHours
    {
        [JsonPropertyName("open_now")]
        public bool? OpenNow { get; set; }

        [JsonPropertyName("weekday_text")]
        public List<string>? WeekdayText { get; set; }
    }

    public class GooglePhoto
    {
        [JsonPropertyName("photo_reference")]
        public string PhotoReference { get; set; } = string.Empty;

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class GoogleReview
    {
        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public long Time { get; set; }
    }
}
