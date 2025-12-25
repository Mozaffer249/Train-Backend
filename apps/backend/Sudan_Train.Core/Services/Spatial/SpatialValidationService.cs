using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Services.Spatial
{
    public class SpatialValidationService : ISpatialValidationService
    {
        private readonly IGeographyService _geographyService;
        private readonly IStationService _stationService;
        private readonly ILogger<SpatialValidationService> _logger;
        private const double DefaultMaxCityRadiusKm = 50.0; // 50km default max radius for cities

        public SpatialValidationService(
            IGeographyService geographyService,
            IStationService stationService,
            ILogger<SpatialValidationService> logger)
        {
            _geographyService = geographyService;
            _stationService = stationService;
            _logger = logger;
        }

        public bool IsPointInPolygon(double lat, double lng, string polygonGeoJson)
        {
            if (string.IsNullOrEmpty(polygonGeoJson))
                return false;

            try
            {
                // Parse GeoJSON polygon
                var coordinates = ParseGeoJsonPolygon(polygonGeoJson);
                if (coordinates == null || coordinates.Count < 3)
                    return false;

                // Ray-casting algorithm
                bool inside = false;
                int j = coordinates.Count - 1;

                for (int i = 0; i < coordinates.Count; i++)
                {
                    if ((coordinates[i].Lat > lat) != (coordinates[j].Lat > lat) &&
                        lng < (coordinates[j].Lng - coordinates[i].Lng) * (lat - coordinates[i].Lat) /
                        (coordinates[j].Lat - coordinates[i].Lat) + coordinates[i].Lng)
                    {
                        inside = !inside;
                    }
                    j = i;
                }

                return inside;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking point in polygon for coordinates: {Lat},{Lng}", lat, lng);
                return false;
            }
        }

        public bool IsPointInBoundingBox(double lat, double lng, double north, double south, double east, double west)
        {
            return lat <= north && lat >= south && lng <= east && lng >= west;
        }

        public double CalculateDistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371; // Earth radius in kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        // Removed: ValidateStateInRegion - no longer needed with simplified hierarchy
        // Removed: ValidateCityInState - no longer needed with simplified hierarchy

        public async Task<bool> ValidateStationInCity(int stationId, int cityId)
        {
            // Check if city exists
            var city = await _geographyService.GetCityByIdAsync(cityId);
            if (city == null)
                return false;

            // For now, return true (coordinate-based validation can be added when DTOs are extended)
            return true;
        }

        // Removed: ValidateCoordinatesForState - no longer needed
        // Removed: ValidateCoordinatesForCity - no longer needed

        public async Task<bool> ValidateCoordinatesForStation(double lat, double lng, int cityId)
        {
            var city = await _geographyService.GetCityByIdAsync(cityId);
            if (city == null)
                return false;

            // For now, return true (boundary validation can be added when city boundaries are available)
            return true;
        }

        // Helper methods
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private List<(double Lat, double Lng)>? ParseGeoJsonPolygon(string geoJsonString)
        {
            try
            {
                using var doc = JsonDocument.Parse(geoJsonString);
                var root = doc.RootElement;

                // GeoJSON polygon format: { "type": "Polygon", "coordinates": [[[lng, lat], [lng, lat], ...]] }
                if (root.TryGetProperty("type", out var type) && type.GetString() == "Polygon")
                {
                    if (root.TryGetProperty("coordinates", out var coordinates) && coordinates.GetArrayLength() > 0)
                    {
                        var outerRing = coordinates[0]; // First array is the outer ring
                        var points = new List<(double Lat, double Lng)>();

                        foreach (var point in outerRing.EnumerateArray())
                        {
                            if (point.GetArrayLength() >= 2)
                            {
                                var lng = point[0].GetDouble();
                                var lat = point[1].GetDouble();
                                points.Add((lat, lng));
                            }
                        }

                        return points;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing GeoJSON polygon: {GeoJson}", geoJsonString);
                return null;
            }
        }
    }
}
