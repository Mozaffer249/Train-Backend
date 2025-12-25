using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Cities.Commands.CreateCity;
using Sudan_Train.Core.Features.Infrastructure.Cities.Commands.UpdateCity;
using Sudan_Train.Core.Features.Infrastructure.Cities.Commands.DeleteCity;
using Sudan_Train.Core.Features.Infrastructure.Cities.Queries.GetAllCities;
using Sudan_Train.Core.Features.Infrastructure.Cities.Queries.GetCityById;
using Sudan_Train.Core.Features.Infrastructure.Cities.Queries.CheckDuplicate;
using Sudan_Train.Core.Features.Infrastructure.Cities.Queries.ValidateLocation;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Infrastructure.InfrastructureBases;
using Sudan_Train.Data.Entity;
using Sudan_Train.Models;

namespace Sudan_Train.Controllers.Infrastructure.Geography
{
    [ApiController]
    [Route(Router.Infrastructure + "/Cities")]
    public class CitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IGenericRepositoryAsync<City> _cityRepository;

        public CitiesController(
            IMediator mediator,
            IGenericRepositoryAsync<City> cityRepository)
        {
            _mediator = mediator;
            _cityRepository = cityRepository;
        }

        /// <summary>
        /// Get all cities
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetCities()
        {
            var response = await _mediator.Send(new GetAllCitiesQuery());
            return Ok(response);
        }

        /// <summary>
        /// Get city by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetCity(int id)
        {
            var response = await _mediator.Send(new GetCityByIdQuery { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Create a new city
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update an existing city
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] UpdateCityCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Delete a city (SuperAdmin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var response = await _mediator.Send(new DeleteCityCommand { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Check if city name is duplicate
        /// </summary>
        [HttpGet("CheckDuplicate")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CheckDuplicate(
            [FromQuery] string nameEn,
            [FromQuery] string nameAr,
            [FromQuery] int? excludeId)
        {
            var response = await _mediator.Send(new CheckCityDuplicateQuery
            {
                NameEn = nameEn,
                NameAr = nameAr,
                ExcludeId = excludeId
            });
            return Ok(response);
        }

        /// <summary>
        /// Search places using Google Places API
        /// </summary>
        [HttpGet("Search")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> SearchPlaces([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { success = false, message = "Search query is required" });
            }

            try
            {
                var apiKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY")
                    ?? throw new InvalidOperationException("Google Maps API key not configured");

                using var httpClient = new HttpClient();
                var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(query)}&types=(cities)&key={apiKey}";
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                return Ok(new { success = true, data = content });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Validate city location and check for duplicates using reverse geocoding
        /// </summary>
        [HttpPost("ValidateLocation")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> ValidateLocation([FromBody] ValidateCityLocationQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Get city boundary data
        /// </summary>
        [HttpGet("{id}/Boundary")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetBoundary(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                return NotFound(new { success = false, message = "City not found" });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    boundaryPolygon = city.BoundaryPolygon,
                    boundingBoxNorth = city.BoundingBoxNorth,
                    boundingBoxSouth = city.BoundingBoxSouth,
                    boundingBoxEast = city.BoundingBoxEast,
                    boundingBoxWest = city.BoundingBoxWest
                }
            });
        }

        /// <summary>
        /// Update city boundary data
        /// </summary>
        [HttpPut("{id}/Boundary")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateBoundary(int id, [FromBody] BoundaryDto dto)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                return NotFound(new { success = false, message = "City not found" });
            }

            city.BoundaryPolygon = dto.BoundaryPolygon;
            city.BoundingBoxNorth = dto.BoundingBoxNorth;
            city.BoundingBoxSouth = dto.BoundingBoxSouth;
            city.BoundingBoxEast = dto.BoundingBoxEast;
            city.BoundingBoxWest = dto.BoundingBoxWest;

            await _cityRepository.UpdateAsync(city);
            await _cityRepository.SaveChangesAsync();

            return Ok(new { success = true, message = "Boundary updated successfully" });
        }
    }
}
