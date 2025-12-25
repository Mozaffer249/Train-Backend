using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Services.Google;
using Sudan_Train.Core.Services.Spatial;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Infrastructure.Geography
{
    /// <summary>
    /// Spatial operations and validation controller
    /// </summary>
    [ApiController]
    [Route(Router.Infrastructure + "/Spatial")]
    [Authorize(Roles = Roles.AdminOrStaff)]
    public class SpatialController : ControllerBase
    {
        private readonly ISpatialValidationService _spatialService;
        private readonly IGoogleGeocodingService _googleGeocodingService;
        private readonly IGooglePlacesService _googlePlacesService;

        public SpatialController(
            ISpatialValidationService spatialService,
            IGoogleGeocodingService googleGeocodingService,
            IGooglePlacesService googlePlacesService)
        {
            _spatialService = spatialService;
            _googleGeocodingService = googleGeocodingService;
            _googlePlacesService = googlePlacesService;
        }

        /// <summary>
        /// Validate if a location is within its parent boundary
        /// </summary>
        [HttpPost("ValidateLocation")]
        public async Task<IActionResult> ValidateLocation([FromBody] ValidateLocationDto dto)
        {
            bool isValid = dto.ParentType.ToLower() switch
            {
                "city" => await _spatialService.ValidateCoordinatesForStation(
                    dto.Latitude, dto.Longitude, dto.ParentId),
                _ => true // Cities don't need parent validation
            };

            return Ok(new
            {
                success = true,
                data = new { isValid, message = isValid ? "Location is valid" : "Location is outside parent boundary" }
            });
        }

        /// <summary>
        /// Get nearby stations using Google Places API
        /// </summary>
        [HttpGet("NearbyStations")]
        public async Task<IActionResult> GetNearbyStations(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] int radiusKm = 25)
        {
            var radiusMeters = radiusKm * 1000;
            var stations = await _googlePlacesService.SearchNearbyStations(lat, lng, radiusMeters);

            return Ok(new
            {
                success = true,
                data = stations
            });
        }

        /// <summary>
        /// Reverse geocode coordinates to get address information
        /// </summary>
        [HttpPost("ReverseGeocode")]
        public async Task<IActionResult> ReverseGeocode([FromBody] ReverseGeocodeDto dto)
        {
            var result = await _googleGeocodingService.ReverseGeocode(dto.Latitude, dto.Longitude);

            if (result == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "Could not reverse geocode coordinates"
                });
            }

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        /// <summary>
        /// Calculate distance between two points
        /// </summary>
        [HttpPost("CalculateDistance")]
        public IActionResult CalculateDistance([FromBody] CalculateDistanceDto dto)
        {
            var distance = _spatialService.CalculateDistanceKm(
                dto.Lat1, dto.Lng1, dto.Lat2, dto.Lng2);

            return Ok(new
            {
                success = true,
                data = new { distanceKm = Math.Round(distance, 2) }
            });
        }
    }

    // DTOs
    public class ValidateLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ParentType { get; set; } = string.Empty; // "region", "state", "city"
        public int ParentId { get; set; }
    }

    public class ReverseGeocodeDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CalculateDistanceDto
    {
        public double Lat1 { get; set; }
        public double Lng1 { get; set; }
        public double Lat2 { get; set; }
        public double Lng2 { get; set; }
    }
}
