using System.Threading.Tasks;

namespace Sudan_Train.Core.Services.Spatial
{
    public interface ISpatialValidationService
    {
        /// <summary>
        /// Check if a point is within a polygon boundary (accurate)
        /// </summary>
        /// <param name="lat">Point latitude</param>
        /// <param name="lng">Point longitude</param>
        /// <param name="polygonGeoJson">GeoJSON polygon string</param>
        /// <returns>True if point is inside polygon</returns>
        bool IsPointInPolygon(double lat, double lng, string polygonGeoJson);

        /// <summary>
        /// Check if a point is within a bounding box (fast)
        /// </summary>
        /// <param name="lat">Point latitude</param>
        /// <param name="lng">Point longitude</param>
        /// <param name="north">Bounding box north coordinate</param>
        /// <param name="south">Bounding box south coordinate</param>
        /// <param name="east">Bounding box east coordinate</param>
        /// <param name="west">Bounding box west coordinate</param>
        /// <returns>True if point is inside bounding box</returns>
        bool IsPointInBoundingBox(double lat, double lng, double north, double south, double east, double west);

        /// <summary>
        /// Calculate distance between two points using Haversine formula
        /// </summary>
        /// <param name="lat1">First point latitude</param>
        /// <param name="lng1">First point longitude</param>
        /// <param name="lat2">Second point latitude</param>
        /// <param name="lng2">Second point longitude</param>
        /// <returns>Distance in kilometers</returns>
        double CalculateDistanceKm(double lat1, double lng1, double lat2, double lng2);

        /// <summary>
        /// Validate that a station's location is within its parent city's boundary or radius
        /// </summary>
        /// <param name="stationId">Station ID to validate</param>
        /// <param name="cityId">Parent city ID</param>
        /// <returns>True if station is within city boundary/radius</returns>
        Task<bool> ValidateStationInCity(int stationId, int cityId);

        /// <summary>
        /// Validate coordinates for a station before entity creation
        /// </summary>
        /// <param name="lat">Station latitude</param>
        /// <param name="lng">Station longitude</param>
        /// <param name="cityId">Parent city ID</param>
        /// <returns>True if coordinates are valid for this city</returns>
        Task<bool> ValidateCoordinatesForStation(double lat, double lng, int cityId);
    }
}
