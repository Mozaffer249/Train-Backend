using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Boarding.Commands.BoardTicket;
using Sudan_Train.Core.Features.Boarding.Commands.MarkNoShow;
using Sudan_Train.Core.Features.Boarding.Commands.ScanTicket;
using Sudan_Train.Core.Features.Boarding.Queries.GetTripManifest;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Operations
{
    // Boarding endpoints. All routes require Admin/SuperAdmin or
    // StaffBoarding. Per-trip station-scope checks happen inside each handler.
    [ApiController]
    [Route(Router.Rule)]
    public class BoardingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BoardingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Manifest = passenger list for a trip, optionally filtered to a boarding station.</summary>
        [HttpGet("Trips/{tripId:int}/Manifest")]
        [Authorize(Roles = Roles.BoardingRoles)]
        public async Task<IActionResult> GetManifest(int tripId, [FromQuery] int? boardingStationId = null)
        {
            var response = await _mediator.Send(new GetTripManifestQuery
            {
                TripId = tripId,
                BoardingStationId = boardingStationId,
            });
            return Ok(response);
        }

        /// <summary>Mark a single ticket Boarded (the per-row manual board action).</summary>
        [HttpPost("Tickets/{ticketId:int}/Board")]
        [Authorize(Roles = Roles.BoardingRoles)]
        public async Task<IActionResult> Board(int ticketId)
        {
            var response = await _mediator.Send(new BoardTicketCommand { TicketId = ticketId });
            return Ok(response);
        }

        /// <summary>Scan a QR payload (camera or paste). Resolves the ticket and boards it atomically.</summary>
        [HttpPost("Tickets/Scan")]
        [Authorize(Roles = Roles.BoardingRoles)]
        public async Task<IActionResult> Scan([FromBody] ScanTicketCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>Flip an Issued ticket to NoShow (passenger didn't show up).</summary>
        [HttpPost("Tickets/{ticketId:int}/NoShow")]
        [Authorize(Roles = Roles.BoardingRoles)]
        public async Task<IActionResult> NoShow(int ticketId)
        {
            var response = await _mediator.Send(new MarkNoShowCommand { TicketId = ticketId });
            return Ok(response);
        }
    }
}
