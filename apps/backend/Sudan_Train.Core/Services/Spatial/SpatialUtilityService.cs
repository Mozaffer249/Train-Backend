using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudan_Train.Core.Services.Spatial
{
    public class SpatialUtilityService
    {
        /// <summary>
        /// Check if a point falls within a GeoJSON polygon using ray-casting algorithm
        /// </summary>
        /// <param name="latitude">Latitude of the point to check</param>
        /// <param name="longitude">Longitude of the point to check</param>
        /// <param name="geoJsonPolygon">GeoJSON polygon string</param>
        /// <returns>True if point is inside polygon, false otherwise</returns>
        public bool IsPointInPolygon(double latitude, double longitude, string geoJsonPolygon)
        {
            try
            {
                var geoJson = JsonSerializer.Deserialize<GeoJsonPolygon>(geoJsonPolygon);
                if (geoJson?.Coordinates == null || geoJson.Coordinates.Length == 0)
                    return false;

                // Get the outer ring (first array)
                var ring = geoJson.Coordinates[0];

                if (ring == null || ring.Length < 3)
                    return false;

                // Ray-casting algorithm
                bool inside = false;
                for (int i = 0, j = ring.Length - 1; i < ring.Length; j = i++)
                {
                    double xi = ring[i][0], yi = ring[i][1];  // lng, lat
                    double xj = ring[j][0], yj = ring[j][1];

                    bool intersect = ((yi > latitude) != (yj > latitude))
                        && (longitude < (xj - xi) * (latitude - yi) / (yj - yi) + xi);

                    if (intersect) inside = !inside;
                }

                return inside;
            }
            catch
            {
                return false;
            }
        }

        private class GeoJsonPolygon
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("coordinates")]
            public double[][][] Coordinates { get; set; } = Array.Empty<double[][]>();
        }
    }
}
