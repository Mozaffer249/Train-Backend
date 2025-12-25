using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Stations.Commands.CreateStation;
using Sudan_Train.Core.Features.Infrastructure.Stations.Commands.UpdateStation;
using Sudan_Train.Core.Features.Infrastructure.Stations.Commands.DeleteStation;
using Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetAllStations;
using Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetStationById;
using Sudan_Train.Core.Features.Infrastructure.Stations.Queries.CheckDuplicate;
using Sudan_Train.Core.Features.Infrastructure.Stations.Queries.ValidateLocation;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Infrastructure.RailwayNetwork
{
    [ApiController]
    [Route(Router.Infrastructure + "/Stations")]
    public class StationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all stations (Public endpoint for customer search)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetStations([FromQuery] GetAllStationsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Get station by ID (Public endpoint)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStation(int id)
        {
            var response = await _mediator.Send(new GetStationByIdQuery { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Create a new station
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateStation([FromBody] CreateStationCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update an existing station
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateStation(int id, [FromBody] UpdateStationCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Delete a station (SuperAdmin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var response = await _mediator.Send(new DeleteStationCommand { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Check if station name is duplicate within a city
        /// </summary>
        [HttpGet("CheckDuplicate")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckDuplicate(
            [FromQuery] string nameEn,
            [FromQuery] string nameAr,
            [FromQuery] int cityId,
            [FromQuery] int? excludeId)
        {
            var response = await _mediator.Send(new CheckStationDuplicateQuery
            {
                NameEn = nameEn,
                NameAr = nameAr,
                CityId = cityId,
                ExcludeId = excludeId
            });
            return Ok(response);
        }

        /// <summary>
        /// Validate station location to ensure it's within city boundaries
        /// </summary>
        [HttpPost("ValidateLocation")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> ValidateLocation([FromBody] ValidateStationLocationQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
