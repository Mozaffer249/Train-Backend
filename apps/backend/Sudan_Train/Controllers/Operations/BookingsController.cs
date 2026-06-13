using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Bookings.Commands.CancelBooking;
using Sudan_Train.Core.Features.Bookings.Commands.CreateBooking;
using Sudan_Train.Core.Features.Bookings.Commands.CreateCounterBooking;
using Sudan_Train.Core.Features.Bookings.Queries.GetAllBookings;
using Sudan_Train.Core.Features.Bookings.Queries.GetBookingById;
using Sudan_Train.Core.Features.Bookings.Queries.GetMyBookings;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Operations
{
    [ApiController]
    [Route(Router.Rule + "Bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Create a booking for a specific trip + boarding/alighting segment + seat.</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>List the current user's bookings (upcoming + past).</summary>
        [HttpGet("Mine")]
        [Authorize]
        public async Task<IActionResult> Mine()
        {
            var response = await _mediator.Send(new GetMyBookingsQuery());
            return Ok(response);
        }

        /// <summary>Fetch a single booking by ID. Customers see their own; admin/staff see any.</summary>
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _mediator.Send(new GetBookingByIdQuery { Id = id });
            return Ok(response);
        }

        /// <summary>Cancel a booking. Owner or admin/staff.</summary>
        [HttpPost("{id:int}/Cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelBookingCommand? body)
        {
            var command = body ?? new CancelBookingCommand();
            command.BookingId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>Paginated list for admin/staff. Optional status filter.</summary>
        [HttpGet]
        [Authorize(Roles = Roles.AnyStaff)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllBookingsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>Counter booking — Staff sells a ticket for a registered customer or a walk-in.</summary>
        [HttpPost("Counter")]
        [Authorize(Roles = Roles.CounterRoles)]
        public async Task<IActionResult> CreateCounter([FromBody] CreateCounterBookingCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
