using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Boarding.Queries.GetTripManifest
{
    public class GetTripManifestQueryHandler
        : ResponseHandler, IRequestHandler<GetTripManifestQuery, Response<TripManifestDto>>
    {
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IStaffAuthorizationService _staffAuth;

        public GetTripManifestQueryHandler(
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStaffAuthorizationService staffAuth,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
            _http = http;
            _staffAuth = staffAuth;
        }

        public async Task<Response<TripManifestDto>> Handle(GetTripManifestQuery request, CancellationToken cancellationToken)
        {
            var trip = await _db.Trip
                .AsNoTracking()
                .Include(t => t.Train)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

            if (trip == null)
                return NotFound<TripManifestDto>("Trip not found.");

            // Station-scope check (skipped for Admin/SuperAdmin inside the helper).
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);
            var roles = _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? new List<string>();

            if (userId > 0 && !await _staffAuth.CanOperateTripAsync(userId, roles, trip.Id))
                return Unauthorized<TripManifestDto>("Not assigned to a station on this trip.");

            var query = _db.BookingPassengers
                .AsNoTracking()
                .Where(bp => bp.TripId == trip.Id && bp.Ticket != null);

            // Station-scope row filter for non-admin staff. Admin / SuperAdmin
            // bypass and may filter by an explicit boardingStationId of their
            // choice (or see all rows when omitted).
            if (userId > 0 && !_staffAuth.IsAdmin(roles))
            {
                var assignedIds = await _staffAuth.GetAssignedStationIdsAsync(userId);
                if (request.BoardingStationId.HasValue)
                {
                    if (!assignedIds.Contains(request.BoardingStationId.Value))
                        return Unauthorized<TripManifestDto>("Boarding station outside your assignment.");
                    query = query.Where(bp => bp.BoardingStationId == request.BoardingStationId.Value);
                }
                else
                {
                    // Multi-station union — show passengers boarding at any of
                    // the caller's assigned stations on this trip.
                    query = query.Where(bp => assignedIds.Contains(bp.BoardingStationId));
                }
            }
            else if (request.BoardingStationId.HasValue)
            {
                query = query.Where(bp => bp.BoardingStationId == request.BoardingStationId.Value);
            }

            var rows = await query
                .Select(bp => new
                {
                    Ticket = bp.Ticket!,
                    BookingId = bp.BookingId,
                    BookingRef = bp.Booking.Reference,
                    PassengerNameEn = bp.Passenger.FullNameEn,
                    PassengerNameAr = bp.Passenger.FullNameAr,
                    IdNumber = bp.Passenger.IdNumber,
                    Seat = bp.TripSeat != null ? bp.TripSeat.Seat.SeatNumber : null,
                    Coach = bp.TripSeat != null ? bp.TripSeat.Seat.Coach.CoachNumber : null,
                    CoachClass = bp.TripSeat != null ? bp.TripSeat.Seat.Coach.Class.ToString() : null,
                    BoardingStationId = bp.BoardingStationId,
                    BoardingStationEn = bp.BoardingStation.NameEn,
                    BoardingStationAr = bp.BoardingStation.NameAr,
                    AlightingStationId = bp.AlightingStationId,
                    AlightingStationEn = bp.AlightingStation.NameEn,
                    AlightingStationAr = bp.AlightingStation.NameAr,
                })
                .ToListAsync(cancellationToken);

            var dto = new TripManifestDto
            {
                TripId = trip.Id,
                TrainNumber = trip.Train?.TrainNumber,
                RouteNameEn = trip.Route?.NameEn,
                RouteNameAr = trip.Route?.NameAr,
                OriginStationEn = trip.Route?.OriginStation?.NameEn,
                OriginStationAr = trip.Route?.OriginStation?.NameAr,
                DestinationStationEn = trip.Route?.DestinationStation?.NameEn,
                DestinationStationAr = trip.Route?.DestinationStation?.NameAr,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Status = trip.Status.ToString(),
                Rows = rows.Select(r => new ManifestRowDto
                {
                    TicketId = r.Ticket.Id,
                    TicketNumber = r.Ticket.TicketNumber,
                    BookingId = r.BookingId,
                    BookingReference = r.BookingRef,
                    PassengerNameEn = r.PassengerNameEn,
                    PassengerNameAr = r.PassengerNameAr,
                    IdNumber = r.IdNumber,
                    SeatNumber = r.Seat,
                    CoachNumber = r.Coach,
                    CoachClass = r.CoachClass,
                    BoardingStationId = r.BoardingStationId,
                    BoardingStationEn = r.BoardingStationEn,
                    BoardingStationAr = r.BoardingStationAr,
                    AlightingStationId = r.AlightingStationId,
                    AlightingStationEn = r.AlightingStationEn,
                    AlightingStationAr = r.AlightingStationAr,
                    Status = r.Ticket.Status.ToString(),
                    BoardedAt = r.Ticket.BoardedAt,
                    BoardedByUserId = r.Ticket.BoardedByUserId,
                }).OrderBy(r => r.CoachNumber).ThenBy(r => r.SeatNumber).ToList(),
            };

            dto.TotalTickets = dto.Rows.Count;
            dto.BoardedCount = dto.Rows.Count(r => r.Status == nameof(TicketStatus.Boarded));
            dto.IssuedCount = dto.Rows.Count(r => r.Status == nameof(TicketStatus.Issued));
            dto.NoShowCount = dto.Rows.Count(r => r.Status == nameof(TicketStatus.NoShow));
            dto.CancelledCount = dto.Rows.Count(r => r.Status == nameof(TicketStatus.Cancelled));

            return Success("Manifest loaded", dto);
        }
    }
}
