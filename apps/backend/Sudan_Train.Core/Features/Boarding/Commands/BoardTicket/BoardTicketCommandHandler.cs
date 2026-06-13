using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Boarding.Commands.BoardTicket
{
    public class BoardTicketCommandHandler
        : ResponseHandler, IRequestHandler<BoardTicketCommand, Response<string>>
    {
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IStaffAuthorizationService _staffAuth;

        public BoardTicketCommandHandler(
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStaffAuthorizationService staffAuth,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
            _http = http;
            _staffAuth = staffAuth;
        }

        public async Task<Response<string>> Handle(BoardTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets
                .Include(t => t.BookingPassenger).ThenInclude(bp => bp.Booking)
                .Include(t => t.BookingPassenger).ThenInclude(bp => bp.Trip)
                .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            if (ticket == null)
                return NotFound<string>("Ticket not found.");

            var bp = ticket.BookingPassenger;
            if (bp == null)
                return BadRequest<string>("Ticket is not linked to a booking.");

            var trip = bp.Trip;

            // Station-scope check.
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);
            var roles = _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? new List<string>();

            if (userId <= 0)
                return Unauthorized<string>("Not authenticated.");

            if (!await _staffAuth.CanOperateTripAsync(userId, roles, trip.Id))
                return Unauthorized<string>("Not assigned to a station on this trip.");

            // Idempotent: already boarded → success.
            if (ticket.Status == TicketStatus.Boarded)
                return Success("Ticket already boarded.", ticket.TicketNumber);

            if (ticket.Status == TicketStatus.Cancelled)
                return BadRequest<string>("Ticket is cancelled.");

            if (ticket.Status == TicketStatus.NoShow)
                return BadRequest<string>("Ticket already marked no-show.");

            // Trip must not be cancelled.
            if (trip.Status == TripStatus.Cancelled)
                return BadRequest<string>("Trip is cancelled.");

            if (bp.Booking.Status != BookingStatus.Confirmed)
                return BadRequest<string>("Booking is not confirmed.");

            ticket.Status = TicketStatus.Boarded;
            ticket.BoardedAt = DateTime.UtcNow;
            ticket.BoardedByUserId = userId;

            await _db.SaveChangesAsync(cancellationToken);

            return Success("Ticket boarded", ticket.TicketNumber);
        }
    }
}
