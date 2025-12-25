using System.Collections.Generic;
using System.Threading.Tasks;
using Sudan_Train.Core.Services.Google.Models;

namespace Sudan_Train.Core.Services.Google
{
    public interface IGooglePlacesService
    {
        /// <summary>
        /// Search for nearby places (e.g., train stations) within a radius
        /// </summary>
        /// <param name="latitude">Center point latitude</param>
        /// <param name="longitude">Center point longitude</param>
        /// <param name="radiusMeters">Search radius in meters</param>
        /// <param name="type">Place type filter (e.g., "train_station", "bus_station")</param>
        /// <returns>List of matching places</returns>
        Task<List<GooglePlaceResult>> SearchNearbyStations(
            double latitude,
            double longitude,
            int radiusMeters,
            string type = "train_station"
        );

        /// <summary>
        /// Get detailed information about a specific place
        /// </summary>
        /// <param name="placeId">Google Place ID</param>
        /// <returns>Detailed place information or null if not found</returns>
        Task<GooglePlaceDetails?> GetPlaceDetails(string placeId);

        /// <summary>
        /// Search for places by text query
        /// </summary>
        /// <param name="query">Search query text</param>
        /// <param name="latitude">Optional: Center point latitude for biasing results</param>
        /// <param name="longitude">Optional: Center point longitude for biasing results</param>
        /// <returns>List of matching places</returns>
        Task<List<GooglePlaceResult>> SearchPlacesByQuery(
            string query,
            double? latitude = null,
            double? longitude = null
        );
    }
}
