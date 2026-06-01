using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CreateTrip;
using Sudan_Train.Core.Features.Infrastructure.Trips.Commands.UpdateTrip;
using Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CancelTrip;
using Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetAllTrips;
using Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetTripById;
using Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetSegmentSeats;
using Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetApplicableFare;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Controllers.Infrastructure.Operations
{
    [ApiController]
    [Route(Router.Infrastructure + "/Trips")]
    public class TripsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all trips (Public endpoint for customer search and booking)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrips([FromQuery] GetAllTripsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Get trip by ID with train, route, and seat availability (Public endpoint)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrip(int id)
        {
            var response = await _mediator.Send(new GetTripByIdQuery { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Create a new trip (auto-initializes TripSeats from train coaches)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update an existing trip
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateTrip(int id, [FromBody] UpdateTripCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Cancel a trip and notify affected passengers
        /// </summary>
        [HttpPut("{id}/Cancel")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CancelTrip(int id)
        {
            var response = await _mediator.Send(new CancelTripCommand { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Get seat availability for a specific boarding→alighting segment of this trip.
        /// A seat is unavailable when it's flagged for maintenance OR when an existing
        /// non-cancelled booking on the same seat overlaps the requested segment.
        /// </summary>
        [HttpGet("{id}/Seats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSegmentSeats(int id, [FromQuery] int boardingStationId, [FromQuery] int alightingStationId)
        {
            var response = await _mediator.Send(new GetSegmentSeatsQuery
            {
                TripId = id,
                BoardingStationId = boardingStationId,
                AlightingStationId = alightingStationId,
            });
            return Ok(response);
        }

        /// <summary>
        /// Resolve the applicable fare for a specific trip + segment + coach class.
        /// Resolution priority: trip-specific override → segment fare → route-level fare.
        /// </summary>
        [HttpGet("{id}/Fare")]
        [AllowAnonymous]
        public async Task<IActionResult> GetApplicableFare(
            int id,
            [FromQuery] int boardingStationId,
            [FromQuery] int alightingStationId,
            [FromQuery] CoachClass? coachClass = null)
        {
            var response = await _mediator.Send(new GetApplicableFareQuery
            {
                TripId = id,
                BoardingStationId = boardingStationId,
                AlightingStationId = alightingStationId,
                CoachClass = coachClass,
            });
            return Ok(response);
        }
    }
}
