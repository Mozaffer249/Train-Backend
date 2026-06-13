using System.Security.Claims;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Boarding.Commands.ScanTicket
{
    public class ScanTicketCommandHandler
        : ResponseHandler, IRequestHandler<ScanTicketCommand, Response<ScanTicketResultDto>>
    {
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IStaffAuthorizationService _staffAuth;

        public ScanTicketCommandHandler(
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStaffAuthorizationService staffAuth,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
            _http = http;
            _staffAuth = staffAuth;
        }

        public async Task<Response<ScanTicketResultDto>> Handle(ScanTicketCommand request, CancellationToken cancellationToken)
        {
            var payload = (request.QrPayload ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(payload))
                return BadRequest<ScanTicketResultDto>("Empty scan payload.");

            // Strategy: try to parse as JSON (the QR format produced at issue
            // time). If parse succeeds, look up by TicketNumber built from
            // (ref + seat). Otherwise treat as a raw ticket number / qr blob.
            string? ticketNumber = null;
            try
            {
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;
                if (root.TryGetProperty("ref", out var refEl) &&
                    root.TryGetProperty("seat", out var seatEl))
                {
                    ticketNumber = $"{refEl.GetString()}-{seatEl.GetString()}";
                }
            }
            catch (JsonException) { /* not JSON → fall through */ }

            var ticket = await _db.Tickets
                .Include(t => t.BookingPassenger).ThenInclude(bp => bp.Booking)
                .Include(t => t.BookingPassenger).ThenInclude(bp => bp.Trip)
                .Include(t => t.BookingPassenger).ThenInclude(bp => bp.Passenger)
                .Include(t => t.BookingPassenger).ThenInclude(bp => bp.TripSeat).ThenInclude(ts => ts!.Seat)
                .FirstOrDefaultAsync(t =>
                    (ticketNumber != null && t.TicketNumber == ticketNumber) ||
                    t.TicketNumber == payload ||
                    t.QrCode == payload, cancellationToken);

            if (ticket == null)
                return NotFound<ScanTicketResultDto>("Ticket not found for the scanned payload.");

            var bp = ticket.BookingPassenger;
            if (bp == null)
                return BadRequest<ScanTicketResultDto>("Ticket is not linked to a booking.");
            var trip = bp.Trip;

            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);
            var roles = _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? new List<string>();

            if (userId <= 0)
                return Unauthorized<ScanTicketResultDto>("Not authenticated.");

            if (!await _staffAuth.CanOperateTripAsync(userId, roles, trip.Id))
                return Unauthorized<ScanTicketResultDto>("Not assigned to a station on this trip.");

            var seatNumber = bp.TripSeat?.Seat?.SeatNumber;
            var passengerName = bp.Passenger?.FullNameEn ?? bp.Passenger?.FullNameAr;

            var resultDto = new ScanTicketResultDto
            {
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                PassengerName = passengerName,
                SeatNumber = seatNumber,
                TripId = trip.Id,
                Status = ticket.Status.ToString(),
            };

            if (ticket.Status == TicketStatus.Boarded)
            {
                resultDto.Status = nameof(TicketStatus.Boarded);
                return Success("Already boarded.", resultDto);
            }
            if (ticket.Status == TicketStatus.Cancelled)
                return BadRequest<ScanTicketResultDto>("Ticket is cancelled.");
            if (ticket.Status == TicketStatus.NoShow)
                return BadRequest<ScanTicketResultDto>("Ticket already marked no-show.");
            if (trip.Status == TripStatus.Cancelled)
                return BadRequest<ScanTicketResultDto>("Trip is cancelled.");
            if (bp.Booking.Status != BookingStatus.Confirmed)
                return BadRequest<ScanTicketResultDto>("Booking is not confirmed.");

            ticket.Status = TicketStatus.Boarded;
            ticket.BoardedAt = DateTime.UtcNow;
            ticket.BoardedByUserId = userId;
            await _db.SaveChangesAsync(cancellationToken);

            resultDto.Status = nameof(TicketStatus.Boarded);
            return Success("Ticket boarded", resultDto);
        }
    }
}
