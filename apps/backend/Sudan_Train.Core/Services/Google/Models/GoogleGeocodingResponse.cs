using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sudan_Train.Core.Services.Google.Models
{
    public class GoogleGeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GoogleResult> Results { get; set; } = new List<GoogleResult>();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }
    }

    public class GoogleResult
    {
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        [JsonPropertyName("formatted_address")]
        public string FormattedAddress { get; set; } = string.Empty;

        [JsonPropertyName("geometry")]
        public GoogleGeometry Geometry { get; set; } = new GoogleGeometry();

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = new List<string>();

        [JsonPropertyName("plus_code")]
        public GooglePlusCode? PlusCode { get; set; }

        [JsonPropertyName("address_components")]
        public List<GoogleAddressComponent> AddressComponents { get; set; } = new List<GoogleAddressComponent>();
    }

    public class GoogleGeometry
    {
        [JsonPropertyName("location")]
        public GoogleLocation Location { get; set; } = new GoogleLocation();

        [JsonPropertyName("viewport")]
        public GoogleViewport? Viewport { get; set; }

        [JsonPropertyName("location_type")]
        public string? LocationType { get; set; }
    }

    public class GoogleLocation
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }

    public class GoogleViewport
    {
        [JsonPropertyName("northeast")]
        public GoogleLocation Northeast { get; set; } = new GoogleLocation();

        [JsonPropertyName("southwest")]
        public GoogleLocation Southwest { get; set; } = new GoogleLocation();
    }

    public class GooglePlusCode
    {
        [JsonPropertyName("global_code")]
        public string? GlobalCode { get; set; }

        [JsonPropertyName("compound_code")]
        public string? CompoundCode { get; set; }
    }

    public class GoogleAddressComponent
    {
        [JsonPropertyName("long_name")]
        public string LongName { get; set; } = string.Empty;

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; } = string.Empty;

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = new List<string>();
    }
}
