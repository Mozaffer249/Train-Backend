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
    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GooglePlacesService> _logger;
        private readonly string _apiKey;
        private readonly bool _enabled;
        private const string NearbySearchUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
        private const string PlaceDetailsUrl = "https://maps.googleapis.com/maps/api/place/details/json";
        private const string TextSearchUrl = "https://maps.googleapis.com/maps/api/place/textsearch/json";

        public GooglePlacesService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GooglePlacesService> logger)
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

        public async Task<List<GooglePlaceResult>> SearchNearbyStations(
            double latitude,
            double longitude,
            int radiusMeters,
            string type = "train_station")
        {
            if (!_enabled || string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Places API is disabled or API key is missing");
                return new List<GooglePlaceResult>();
            }

            try
            {
                var location = $"{latitude},{longitude}";
                var url = $"{NearbySearchUrl}?location={location}&radius={radiusMeters}&type={type}&key={_apiKey}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google Places API request failed with status: {StatusCode}", response.StatusCode);
                    return new List<GooglePlaceResult>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GooglePlacesResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.Status != "OK")
                {
                    if (result?.Status == "ZERO_RESULTS")
                    {
                        _logger.LogInformation("No places found for location: {Latitude}, {Longitude}", latitude, longitude);
                        return new List<GooglePlaceResult>();
                    }

                    _logger.LogWarning("Google Places API returned status: {Status}, Message: {Message}",
                        result?.Status, result?.ErrorMessage);
                    return new List<GooglePlaceResult>();
                }

                return result.Results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Places API for nearby search at: {Latitude},{Longitude}",
                    latitude, longitude);
                return new List<GooglePlaceResult>();
            }
        }

        public async Task<GooglePlaceDetails?> GetPlaceDetails(string placeId)
        {
            if (!_enabled || string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Places API is disabled or API key is missing");
                return null;
            }

            try
            {
                var url = $"{PlaceDetailsUrl}?place_id={placeId}&fields=name,formatted_address,geometry,types,business_status,formatted_phone_number,website,opening_hours,photos,reviews,rating,user_ratings_total&key={_apiKey}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google Places API request failed with status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GooglePlaceDetailsResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.Status != "OK")
                {
                    _logger.LogWarning("Google Places API returned status: {Status}, Message: {Message}",
                        result?.Status, result?.ErrorMessage);
                    return null;
                }

                return result.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Places API for place details: {PlaceId}", placeId);
                return null;
            }
        }

        public async Task<List<GooglePlaceResult>> SearchPlacesByQuery(
            string query,
            double? latitude = null,
            double? longitude = null)
        {
            if (!_enabled || string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Places API is disabled or API key is missing");
                return new List<GooglePlaceResult>();
            }

            try
            {
                var url = $"{TextSearchUrl}?query={Uri.EscapeDataString(query)}&key={_apiKey}";

                if (latitude.HasValue && longitude.HasValue)
                {
                    var location = $"{latitude.Value},{longitude.Value}";
                    url += $"&location={location}&radius=50000"; // 50km radius for biasing
                }

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google Places API request failed with status: {StatusCode}", response.StatusCode);
                    return new List<GooglePlaceResult>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GooglePlacesResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.Status != "OK")
                {
                    if (result?.Status == "ZERO_RESULTS")
                    {
                        _logger.LogInformation("No places found for query: {Query}", query);
                        return new List<GooglePlaceResult>();
                    }

                    _logger.LogWarning("Google Places API returned status: {Status}, Message: {Message}",
                        result?.Status, result?.ErrorMessage);
                    return new List<GooglePlaceResult>();
                }

                return result.Results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Places API for text search: {Query}", query);
                return new List<GooglePlaceResult>();
            }
        }
    }
}
