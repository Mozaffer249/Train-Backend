using System.Collections.Generic;
using System.Threading.Tasks;
using Sudan_Train.Core.Services.Google.Models;

namespace Sudan_Train.Core.Services.Google
{
    public interface IGoogleGeocodingService
    {
        /// <summary>
        /// Geocode an address to get place details including coordinates
        /// </summary>
        /// <param name="address">Full address to geocode</param>
        /// <returns>Google result with place details or null if not found</returns>
        Task<GoogleResult?> GeocodeAddress(string address);

        /// <summary>
        /// Search for places by query and type
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="type">Place type filter (e.g., "locality", "administrative_area_level_1")</param>
        /// <returns>List of matching places</returns>
        Task<List<GoogleResult>> SearchPlaces(string query, string? type = null);

        /// <summary>
        /// Reverse geocode coordinates to get address
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>Google result with address details or null if not found</returns>
        Task<GoogleResult?> ReverseGeocode(double latitude, double longitude);

        /// <summary>
        /// Extract boundary information from geocode result
        /// </summary>
        /// <param name="result">Google geocode result</param>
        /// <returns>Tuple containing polygon, north, south, east, west boundaries</returns>
        (string? polygon, double? north, double? south, double? east, double? west) ExtractBoundaries(GoogleResult result);
    }
}
