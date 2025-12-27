using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sudan_Train.Core.Services.Google.Models;

namespace Sudan_Train.Core.Services.Google
{
    public class GoogleGeocodingService : IGoogleGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleGeocodingService> _logger;
        private readonly string _apiKey;
        private readonly bool _enabled;
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";

        public GoogleGeocodingService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GoogleGeocodingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["Google:ApiKey"] ?? string.Empty;
            _enabled = configuration.GetValue<bool>("Google:EnableSeeding", false);

            if (string.IsNullOrEmpty(_apiKey) && _enabled)
            {
                _logger.LogWarning("Google API key is not configured but service is enabled");
            }
        }

        public async Task<GoogleResult?> GeocodeAddress(string address)
        {
            if (!_enabled || string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Geocoding is disabled or API key is missing");
                return null;
            }

            try
            {
                var url = $"{BaseUrl}?address={Uri.EscapeDataString(address)}&key={_apiKey}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google API request failed with status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GoogleGeocodingResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.Status != "OK")
                {
                    _logger.LogWarning("Google API returned status: {Status}, Message: {Message}",
                        result?.Status, result?.ErrorMessage);
                    return null;
                }

                return result.Results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Geocoding API for address: {Address}", address);
                return null;
            }
        }

        public async Task<List<GoogleResult>> SearchPlaces(string query, string? type = null)
        {
            if (!_enabled || string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Geocoding is disabled or API key is missing");
                return new List<GoogleResult>();
            }

            try
            {
                var url = $"{BaseUrl}?address={Uri.EscapeDataString(query)}&key={_apiKey}";

                if (!string.IsNullOrEmpty(type))
                {
                    url += $"&components=country:SD&type={type}";
                }

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google API request failed with status: {StatusCode}", response.StatusCode);
                    return new List<GoogleResult>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GoogleGeocodingResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.Status != "OK")
                {
                    _logger.LogWarning("Google API returned status: {Status}, Message: {Message}",
                        result?.Status, result?.ErrorMessage);
                    return new List<GoogleResult>();
                }

                // Filter by type if specified
                if (!string.IsNullOrEmpty(type))
                {
                    return result.Results
                        .Where(r => r.Types.Contains(type, StringComparer.OrdinalIgnoreCase))
                        .ToList();
                }

                return result.Results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Geocoding API for query: {Query}", query);
                return new List<GoogleResult>();
            }
        }

        public async Task<GoogleResult?> ReverseGeocode(double latitude, double longitude)
        {
            if (!_enabled || string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Geocoding is disabled or API key is missing");
                return null;
            }

            try
            {
                var latlng = $"{latitude},{longitude}";
                var url = $"{BaseUrl}?latlng={latlng}&key={_apiKey}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google API request failed with status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GoogleGeocodingResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.Status != "OK")
                {
                    _logger.LogWarning("Google API returned status: {Status}, Message: {Message}",
                        result?.Status, result?.ErrorMessage);
                    return null;
                }

                return result.Results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Reverse Geocoding API for coordinates: {Lat},{Lng}",
                    latitude, longitude);
                return null;
            }
        }

        public (string? polygon, double? north, double? south, double? east, double? west)
            ExtractBoundaries(GoogleResult result)
        {
            var viewport = result.Geometry.Viewport ?? result.Geometry.Bounds;

            if (viewport == null)
                return (null, null, null, null, null);

            // Create polygon from viewport (rectangle)
            var polygon = CreatePolygonFromViewport(viewport);

            return (
                polygon,
                viewport.Northeast.Lat,
                viewport.Southwest.Lat,
                viewport.Northeast.Lng,
                viewport.Southwest.Lng
            );
        }

        private string CreatePolygonFromViewport(GoogleViewport viewport)
        {
            // Create GeoJSON polygon from bounding box
            var ne = viewport.Northeast;
            var sw = viewport.Southwest;

            var polygon = new
            {
                type = "Polygon",
                coordinates = new[]
                {
                    new[]
                    {
                        new[] { sw.Lng, ne.Lat }, // NW
                        new[] { ne.Lng, ne.Lat }, // NE
                        new[] { ne.Lng, sw.Lat }, // SE
                        new[] { sw.Lng, sw.Lat }, // SW
                        new[] { sw.Lng, ne.Lat }  // Close polygon
                    }
                }
            };

            return JsonSerializer.Serialize(polygon);
        }
    }
}
