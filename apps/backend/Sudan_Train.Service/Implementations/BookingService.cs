using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDBContext _db;
        private readonly IFareService _fareService;

        public BookingService(ApplicationDBContext db, IFareService fareService)
        {
            _db = db;
            _fareService = fareService;
        }

        public async Task<BookingCreationResult> CreateBookingAsync(CreateBookingInput input)
        {
            // ---- Validate input shape ----
            if (input.Passengers == null || input.Passengers.Count == 0)
                return new BookingCreationResult { Invalid = true, Error = "At least one passenger is required." };

            // No duplicate seat in the same payload.
            var seatIds = input.Passengers.Select(p => p.SeatId).ToList();
            if (seatIds.Distinct().Count() != seatIds.Count)
                return new BookingCreationResult { Invalid = true, Error = "Duplicate seat in booking." };

            // ---- Load everything we need in one round-trip ----
            var trip = await _db.Trip
                .Include(t => t.Train)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.Route).ThenInclude(r => r.RouteStations).ThenInclude(rs => rs.Station)
                .Include(t => t.TripSeats).ThenInclude(ts => ts.Seat).ThenInclude(s => s.Coach)
                .FirstOrDefaultAsync(t => t.Id == input.TripId);

            if (trip == null)
                return new BookingCreationResult { NotFound = true, Error = "Trip not found." };

            if (trip.Status == "Cancelled" || trip.Status == "Completed")
                return new BookingCreationResult { Invalid = true, Error = $"Cannot book on a {trip.Status} trip." };

            // ---- Resolve stop orders on this trip's route (shared across all seats) ----
            var bOrder = TripService.StopOrderOnRoute(trip.Route, input.BoardingStationId);
            var aOrder = TripService.StopOrderOnRoute(trip.Route, input.AlightingStationId);
            if (bOrder == null || aOrder == null)
                return new BookingCreationResult { Invalid = true, Error = "Boarding or alighting station is not on this route." };
            if (aOrder.Value <= bOrder.Value)
                return new BookingCreationResult { Invalid = true, Error = "Alighting station must come after the boarding station." };

            // ---- Resolve each seat + its fare. Fares are cached per CoachClass.
            var resolved = new List<(PassengerSeatInput ps, TripSeat tripSeat, FareDto fareDto, FareBreakdownDto breakdown)>();
            var fareCache = new Dictionary<CoachClass, FareDto>();

            foreach (var ps in input.Passengers)
            {
                var tripSeat = trip.TripSeats.FirstOrDefault(ts => ts.SeatId == ps.SeatId);
                if (tripSeat == null)
                    return new BookingCreationResult { NotFound = true, Error = $"Seat {ps.SeatId} is not on this trip." };
                if (tripSeat.Status == SeatStatus.Maintenance)
                    return new BookingCreationResult { Conflict = true, Error = $"Seat {tripSeat.Seat?.SeatNumber} is out of service." };

                // Use the seat's actual coach class (more authoritative than client claim).
                var actualClass = tripSeat.Seat?.Coach?.Class ?? ps.CoachClass;

                if (!fareCache.TryGetValue(actualClass, out var fareDto))
                {
                    var f = await _fareService.GetApplicableFareAsync(
                        routeId: trip.RouteId,
                        originStationId: input.BoardingStationId,
                        destinationStationId: input.AlightingStationId,
                        tripId: trip.Id,
                        coachClass: actualClass);

                    if (f == null || f.Breakdown == null)
                        return new BookingCreationResult { Invalid = true, Error = $"No fare configured for {actualClass} on this segment." };

                    fareCache[actualClass] = f;
                    fareDto = f;
                }

                resolved.Add((ps, tripSeat, fareDto, fareDto.Breakdown!));
            }

            // ---- Race-resistant seat-availability re-check across all chosen seats ----
            var tripSeatIds = resolved.Select(r => r.tripSeat.Id).ToList();
            var conflicting = await _db.BookingPassengers
                .Where(bp => bp.TripId == trip.Id
                          && tripSeatIds.Contains(bp.TripSeatId!.Value)
                          && bp.Booking.Status != BookingStatus.Cancelled)
                .Select(bp => new { bp.TripSeatId, bp.BoardingStationId, bp.AlightingStationId })
                .ToListAsync();

            foreach (var c in conflicting)
            {
                var cBOrder = TripService.StopOrderOnRoute(trip.Route, c.BoardingStationId);
                var cAOrder = TripService.StopOrderOnRoute(trip.Route, c.AlightingStationId);
                if (cBOrder == null || cAOrder == null) continue;
                if (bOrder.Value < cAOrder.Value && cBOrder.Value < aOrder.Value)
                {
                    var clash = resolved.First(r => r.tripSeat.Id == c.TripSeatId);
                    return new BookingCreationResult { Conflict = true, Error = $"Seat {clash.tripSeat.Seat?.SeatNumber} is no longer available for this segment." };
                }
            }

            // ---- Upsert Passenger records — one per ID number ----
            var passengerByIdNumber = new Dictionary<string, Passenger>();
            foreach (var (ps, _, _, _) in resolved)
            {
                if (passengerByIdNumber.ContainsKey(ps.Passenger.IdNumber)) continue;
                var existing = await _db.Passengers.FirstOrDefaultAsync(p =>
                    p.IdNumber == ps.Passenger.IdNumber &&
                    (input.UserId == null || p.UserId == input.UserId));
                if (existing == null)
                {
                    existing = new Passenger
                    {
                        UserId = input.UserId,
                        FullNameEn = ps.Passenger.FullNameEn,
                        FullNameAr = ps.Passenger.FullNameAr,
                        IdNumber = ps.Passenger.IdNumber,
                        Gender = ps.Passenger.Gender,
                        Nationality = ps.Passenger.Nationality,
                    };
                    _db.Passengers.Add(existing);
                    await _db.SaveChangesAsync();
                }
                passengerByIdNumber[ps.Passenger.IdNumber] = existing;
            }

            // ---- Insert Booking + N BookingPassengers + Payment + N Tickets atomically ----
            var totalAmount = resolved.Sum(r => r.breakdown.Total);
            var primaryCurrency = resolved[0].fareDto.Currency;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var booking = new Booking
                {
                    Reference = GenerateBookingRef(),
                    UserId = input.UserId,
                    Status = BookingStatus.Pending,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                };
                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();

                var bookingPassengers = new List<BookingPassenger>();
                foreach (var (ps, tripSeat, fareDto, breakdown) in resolved)
                {
                    var passenger = passengerByIdNumber[ps.Passenger.IdNumber];
                    var bp = new BookingPassenger
                    {
                        BookingId = booking.Id,
                        PassengerId = passenger.Id,
                        TripId = trip.Id,
                        TripSeatId = tripSeat.Id,
                        FareId = fareDto.Id,
                        Price = breakdown.Total,
                        BoardingStationId = input.BoardingStationId,
                        AlightingStationId = input.AlightingStationId,
                    };
                    _db.BookingPassengers.Add(bp);
                    bookingPassengers.Add(bp);
                }
                await _db.SaveChangesAsync();

                // Mock payment authorisation for the full booking — flips to Completed.
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    Method = input.PaymentMethod,
                    Amount = totalAmount,
                    Currency = primaryCurrency,
                    Status = PaymentStatus.Completed,
                    CardLast4 = input.CardLast4,
                    Reference = $"MOCK-{booking.Reference}",
                    CreatedAt = DateTime.UtcNow,
                };
                _db.Payments.Add(payment);
                booking.Status = BookingStatus.Confirmed;

                // One Ticket per BookingPassenger.
                for (int i = 0; i < resolved.Count; i++)
                {
                    var (_, tripSeat, _, _) = resolved[i];
                    var bp = bookingPassengers[i];
                    var seatNumber = tripSeat.Seat?.SeatNumber ?? "?";
                    var qrPayload = JsonSerializer.Serialize(new
                    {
                        @ref = booking.Reference,
                        tripId = trip.Id,
                        boarding = input.BoardingStationId,
                        alighting = input.AlightingStationId,
                        seat = seatNumber,
                        departure = trip.DepartureTime,
                    });
                    _db.Tickets.Add(new Ticket
                    {
                        BookingPassengerId = bp.Id,
                        TicketNumber = $"{booking.Reference}-{seatNumber}",
                        QrCode = qrPayload,
                        IssuedAt = DateTime.UtcNow,
                        Status = "Issued",
                    });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                var dto = await BuildBookingDtoAsync(booking.Id);
                return new BookingCreationResult { Booking = dto };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int? userId, bool isAdmin, string? reason)
        {
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return false;
            if (!isAdmin && booking.UserId != userId) return false;
            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed) return false;

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = reason;
            booking.CancelledBy = userId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<BookingDto?> GetByIdAsync(int bookingId, int? userId, bool isAdmin)
        {
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return null;
            if (!isAdmin && booking.UserId != userId) return null;
            return await BuildBookingDtoAsync(bookingId);
        }

        public async Task<List<BookingDto>> GetMineAsync(int userId)
        {
            var ids = await _db.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => b.Id)
                .ToListAsync();

            var result = new List<BookingDto>();
            foreach (var id in ids)
            {
                var dto = await BuildBookingDtoAsync(id);
                if (dto != null) result.Add(dto);
            }
            return result;
        }

        public async Task<List<BookingDto>> GetAllAsync(BookingListParams query)
        {
            var q = _db.Bookings.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Status) &&
                Enum.TryParse<BookingStatus>(query.Status, ignoreCase: true, out var st))
            {
                q = q.Where(b => b.Status == st);
            }
            if (query.UserId.HasValue)
                q = q.Where(b => b.UserId == query.UserId);

            var ids = await q
                .OrderByDescending(b => b.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(b => b.Id)
                .ToListAsync();

            var result = new List<BookingDto>();
            foreach (var id in ids)
            {
                var dto = await BuildBookingDtoAsync(id);
                if (dto != null) result.Add(dto);
            }
            return result;
        }

        // ---------- Helpers ----------
        private async Task<BookingDto?> BuildBookingDtoAsync(int bookingId)
        {
            var booking = await _db.Bookings
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Passenger).ThenInclude(p => p!.User)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Trip).ThenInclude(t => t.Train)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Trip).ThenInclude(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Trip).ThenInclude(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Trip).ThenInclude(t => t.Route).ThenInclude(r => r.RouteStations).ThenInclude(rs => rs.Station)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.BoardingStation)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.AlightingStation)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.TripSeat).ThenInclude(ts => ts!.Seat).ThenInclude(s => s.Coach)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Fare)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Ticket)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return null;
            var bps = booking.BookingPassengers.ToList();
            if (bps.Count == 0) return null;

            // Primary entry — first passenger. Segment + trip metadata are
            // shared across every passenger on the booking.
            var primary = bps[0];
            var route = primary.Trip.Route;
            var (departure, arrival) = ComputeSegmentTimes(primary.Trip, primary.BoardingStationId, primary.AlightingStationId);

            FareBreakdownDto MakeBreakdown(BookingPassenger bp) =>
                bp.Fare != null
                    ? FareService.BuildBreakdown(bp.Fare)
                    : new FareBreakdownDto { BasePrice = bp.Price, Total = bp.Price, Currency = "SDG" };

            BookingPassengerInfoDto MakePassengerInfo(BookingPassenger bp) => new()
            {
                FullNameEn = bp.Passenger.FullNameEn,
                FullNameAr = bp.Passenger.FullNameAr,
                IdNumber = bp.Passenger.IdNumber,
                Phone = bp.Passenger.User?.PhoneNumber,
                Email = bp.Passenger.User?.Email,
                Gender = bp.Passenger.Gender,
                Nationality = bp.Passenger.Nationality,
            };

            TicketInfoDto? MakeTicketInfo(BookingPassenger bp) =>
                bp.Ticket == null ? null : new TicketInfoDto
                {
                    TicketNumber = bp.Ticket.TicketNumber,
                    QrPayload = bp.Ticket.QrCode,
                    Status = bp.Ticket.Status,
                };

            var primaryBreakdown = MakeBreakdown(primary);
            var primaryCoach = primary.TripSeat?.Seat?.Coach;

            var passengerDetails = bps.Select(bp =>
            {
                var seatNum = bp.TripSeat?.Seat?.SeatNumber ?? "?";
                var coachCls = bp.TripSeat?.Seat?.Coach?.Class ?? CoachClass.Second;
                return new BookingPassengerDetailDto
                {
                    Passenger = MakePassengerInfo(bp),
                    SeatNumber = seatNum,
                    CoachClass = coachCls.ToString(),
                    Price = bp.Price,
                    Ticket = MakeTicketInfo(bp),
                };
            }).ToList();

            return new BookingDto
            {
                Id = booking.Id,
                BookingRef = booking.Reference,
                TripId = primary.TripId,
                TrainName = primary.Trip.Train.NameAr ?? primary.Trip.Train.NameEn ?? "",
                RouteName = route.NameAr ?? route.NameEn ?? "",
                BoardingStationId = primary.BoardingStationId,
                AlightingStationId = primary.AlightingStationId,
                BoardingStationName = primary.BoardingStation.NameAr ?? primary.BoardingStation.NameEn ?? "",
                AlightingStationName = primary.AlightingStation.NameAr ?? primary.AlightingStation.NameEn ?? "",
                DepartureTime = departure,
                ArrivalTime = arrival,
                CoachClass = (primaryCoach?.Class ?? CoachClass.Second).ToString(),
                SeatNumber = primary.TripSeat?.Seat?.SeatNumber ?? "?",
                Passenger = MakePassengerInfo(primary),
                Ticket = MakeTicketInfo(primary),
                Passengers = passengerDetails,
                BasePrice = primaryBreakdown.BasePrice,
                Total = bps.Sum(bp => bp.Price),
                Currency = primaryBreakdown.Currency,
                Breakdown = primaryBreakdown,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt,
            };
        }

        private static (DateTime departure, DateTime arrival) ComputeSegmentTimes(Trip trip, int boardingStationId, int alightingStationId)
        {
            var route = trip.Route;
            DateTime departure = trip.DepartureTime;
            DateTime arrival = trip.ArrivalTime;

            // Boarding time: trip departure for origin; trip departure + DepartureOffset for an intermediate.
            if (boardingStationId == route.OriginStationId)
            {
                departure = trip.DepartureTime;
            }
            else
            {
                var boardingRs = route.RouteStations.FirstOrDefault(rs => rs.StationId == boardingStationId);
                if (boardingRs != null)
                {
                    var offset = boardingRs.DepartureOffset ?? boardingRs.ArrivalOffset ?? TimeSpan.Zero;
                    departure = trip.DepartureTime + offset;
                }
            }

            // Alighting time: intermediate ArrivalOffset, or trip arrival for the destination.
            if (alightingStationId == route.DestinationStationId)
            {
                arrival = trip.ArrivalTime;
            }
            else
            {
                var alightingRs = route.RouteStations.FirstOrDefault(rs => rs.StationId == alightingStationId);
                if (alightingRs != null)
                {
                    var offset = alightingRs.ArrivalOffset ?? alightingRs.DepartureOffset ?? TimeSpan.Zero;
                    arrival = trip.DepartureTime + offset;
                }
            }

            return (departure, arrival);
        }

        private static string GenerateBookingRef()
        {
            var year = DateTime.UtcNow.Year;
            var rand = Random.Shared.Next(100000, 999999);
            return $"SD-{year}-{rand}";
        }
    }
}
