using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripSeatRepository _tripSeatRepository;
        private readonly ITrainRepository _trainRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly ICoachRepository _coachRepository;
        private readonly ApplicationDBContext _db;
        private readonly IBookingNotificationService _bookingNotifications;

        public TripService(
            ITripRepository tripRepository,
            ITripSeatRepository tripSeatRepository,
            ITrainRepository trainRepository,
            IRouteRepository routeRepository,
            ICoachRepository coachRepository,
            ApplicationDBContext db,
            IBookingNotificationService bookingNotifications)
        {
            _tripRepository = tripRepository;
            _tripSeatRepository = tripSeatRepository;
            _trainRepository = trainRepository;
            _routeRepository = routeRepository;
            _coachRepository = coachRepository;
            _db = db;
            _bookingNotifications = bookingNotifications;
        }

        public async Task<TripDto> CreateTripAsync(int trainId, int routeId, DateTime departureTime, DateTime arrivalTime)
        {
            var trip = new Trip
            {
                TrainId = trainId,
                RouteId = routeId,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Status = TripStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            await _tripRepository.AddAsync(trip);

            // Auto-initialize TripSeats
            await InitializeTripSeatsAsync(trip.Id, trainId);

            // Load full details for DTO
            var tripDetails = await _tripRepository.GetTableNoTracking()
                .Include(t => t.Train)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.TripSeats)
                .FirstOrDefaultAsync(t => t.Id == trip.Id);

            return new TripDto
            {
                Id = tripDetails!.Id,
                TrainId = tripDetails.TrainId,
                TrainNumber = tripDetails.Train.TrainNumber,
                TrainName = tripDetails.Train.NameAr ?? tripDetails.Train.NameEn ?? "",
                RouteId = tripDetails.RouteId,
                RouteName = tripDetails.Route.NameAr ?? tripDetails.Route.NameEn ?? "",
                OriginStation = tripDetails.Route.OriginStation.NameAr ?? tripDetails.Route.OriginStation.NameEn ?? "",
                DestinationStation = tripDetails.Route.DestinationStation.NameAr ?? tripDetails.Route.DestinationStation.NameEn ?? "",
                DepartureTime = tripDetails.DepartureTime,
                ArrivalTime = tripDetails.ArrivalTime,
                Status = tripDetails.Status.ToString(),
                TotalSeats = tripDetails.TripSeats.Count,
                AvailableSeats = tripDetails.TripSeats.Count(ts => ts.Status == SeatStatus.Available),
                BookedSeats = tripDetails.TripSeats.Count(ts => ts.Status == SeatStatus.Occupied)
            };
        }

        public async Task<TripDto?> GetTripByIdAsync(int id)
        {
            var trip = await _tripRepository.GetTableNoTracking()
                .Include(t => t.Train)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.TripSeats)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return null;

            return new TripDto
            {
                Id = trip.Id,
                TrainId = trip.TrainId,
                TrainNumber = trip.Train.TrainNumber,
                TrainName = trip.Train.NameAr ?? trip.Train.NameEn ?? "",
                RouteId = trip.RouteId,
                RouteName = trip.Route.NameAr ?? trip.Route.NameEn ?? "",
                OriginStation = trip.Route.OriginStation.NameAr ?? trip.Route.OriginStation.NameEn ?? "",
                DestinationStation = trip.Route.DestinationStation.NameAr ?? trip.Route.DestinationStation.NameEn ?? "",
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Status = trip.Status.ToString(),
                TotalSeats = trip.TripSeats.Count,
                AvailableSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Available),
                BookedSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Occupied)
            };
        }

        public async Task<List<TripDto>> GetAllTripsAsync(
            DateTime? date = null,
            int? routeId = null,
            string? status = null,
            List<int>? assignedStationIds = null,
            bool upcomingOnly = false)
        {
            var query = _tripRepository.GetTableNoTracking()
                .Include(t => t.Train)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.TripSeats)
                .AsQueryable();

            // When the caller is station-staff, include RouteStations so we
            // can read each station's offset for the per-station "upcoming"
            // filter below.
            var hasStationScope = assignedStationIds != null && assignedStationIds.Count > 0;
            if (hasStationScope)
                query = query.Include(t => t.Route).ThenInclude(r => r.RouteStations);

            if (date.HasValue)
            {
                var startOfDay = date.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(t => t.DepartureTime >= startOfDay && t.DepartureTime < endOfDay);
            }

            if (routeId.HasValue)
                query = query.Where(t => t.RouteId == routeId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TripStatus>(status, true, out var statusEnum))
                query = query.Where(t => t.Status == statusEnum);

            // Trip departure times are stored as naive local clock-on-the-
            // wall values (Sudan time), so compare against DateTime.Now
            // (server local time, configured to UTC+3). Using DateTime.UtcNow
            // here would let June-1-23:30 trips through at June-2-02:30 local.
            var now = DateTime.Now;

            // When the caller has no station scope (admin / customer search /
            // anonymous), the simple `DepartureTime > now` is the right
            // upcoming filter. Station-scoped callers compute their local
            // station departure in memory below — defer the upcoming check.
            if (upcomingOnly && !hasStationScope)
                query = query.Where(t => t.DepartureTime > now);

            // Station scope: trip's route must touch one of the caller's assigned
            // stations (origin, destination, or any intermediate RouteStation).
            if (hasStationScope)
            {
                query = query.Where(t =>
                    assignedStationIds!.Contains(t.Route.OriginStationId) ||
                    assignedStationIds!.Contains(t.Route.DestinationStationId) ||
                    t.Route.RouteStations.Any(rs => assignedStationIds!.Contains(rs.StationId)));
            }

            var trips = await query
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

            // For station-staff with upcomingOnly: keep a trip only if the
            // train hasn't departed the agent's station yet. The agent's
            // local departure = trip.DepartureTime + (station's offset on
            // this route). Origin → offset 0; intermediates → DepartureOffset
            // (or ArrivalOffset fallback); destination → no departure, so it
            // never counts as "still upcoming for boarding".
            if (upcomingOnly && hasStationScope)
            {
                trips = trips.Where(t =>
                {
                    foreach (var stationId in assignedStationIds!)
                    {
                        TimeSpan? offset = null;
                        if (stationId == t.Route.OriginStationId)
                        {
                            offset = TimeSpan.Zero;
                        }
                        else if (stationId == t.Route.DestinationStationId)
                        {
                            // Trip ends at destination — nothing to board there.
                            continue;
                        }
                        else
                        {
                            var rs = t.Route.RouteStations.FirstOrDefault(x => x.StationId == stationId);
                            if (rs != null)
                                offset = rs.DepartureOffset ?? rs.ArrivalOffset;
                        }

                        if (offset.HasValue && t.DepartureTime + offset.Value > now)
                            return true;
                    }
                    return false;
                }).ToList();
            }

            return trips.Select(t => new TripDto
            {
                Id = t.Id,
                TrainId = t.TrainId,
                TrainNumber = t.Train.TrainNumber,
                TrainName = t.Train.NameAr ?? t.Train.NameEn ?? "",
                RouteId = t.RouteId,
                RouteName = t.Route.NameAr ?? t.Route.NameEn ?? "",
                OriginStation = t.Route.OriginStation.NameAr ?? t.Route.OriginStation.NameEn ?? "",
                DestinationStation = t.Route.DestinationStation.NameAr ?? t.Route.DestinationStation.NameEn ?? "",
                DepartureTime = t.DepartureTime,
                ArrivalTime = t.ArrivalTime,
                Status = t.Status.ToString(),
                TotalSeats = t.TripSeats.Count,
                AvailableSeats = t.TripSeats.Count(ts => ts.Status == SeatStatus.Available),
                BookedSeats = t.TripSeats.Count(ts => ts.Status == SeatStatus.Occupied)
            }).ToList();
        }

        public async Task<TripDto> UpdateTripAsync(int id, DateTime departureTime, DateTime arrivalTime, string status)
        {
            var trip = await _tripRepository.GetTableNoTracking()
                .Include(t => t.Train)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.TripSeats)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                throw new KeyNotFoundException($"Trip with ID {id} not found");

            trip.DepartureTime = departureTime;
            trip.ArrivalTime = arrivalTime;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TripStatus>(status, true, out var statusEnum))
                trip.Status = statusEnum;
            trip.UpdatedAt = DateTime.UtcNow;

            await _tripRepository.UpdateAsync(trip);

            return new TripDto
            {
                Id = trip.Id,
                TrainId = trip.TrainId,
                TrainNumber = trip.Train.TrainNumber,
                TrainName = trip.Train.NameAr ?? trip.Train.NameEn ?? "",
                RouteId = trip.RouteId,
                RouteName = trip.Route.NameAr ?? trip.Route.NameEn ?? "",
                OriginStation = trip.Route.OriginStation.NameAr ?? trip.Route.OriginStation.NameEn ?? "",
                DestinationStation = trip.Route.DestinationStation.NameAr ?? trip.Route.DestinationStation.NameEn ?? "",
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Status = trip.Status.ToString(),
                TotalSeats = trip.TripSeats.Count,
                AvailableSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Available),
                BookedSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Occupied)
            };
        }

        public async Task<bool> CancelTripAsync(int id)
        {
            // Back-compat shim: route plain CancelTrip through the cascade
            // variant. actorUserId = 0 means "system" — Audit fields stay null
            // when no caller supplied an ID.
            return await CancelTripWithCascadeAsync(id, 0, null);
        }

        public async Task<bool> MarkDepartedAsync(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null) return false;
            if (trip.Status != TripStatus.Scheduled && trip.Status != TripStatus.Delayed)
                return false;

            trip.Status = TripStatus.Departed;
            trip.UpdatedAt = DateTime.UtcNow;
            await _tripRepository.UpdateAsync(trip);
            return true;
        }

        public async Task<bool> MarkArrivedAsync(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null) return false;
            if (trip.Status != TripStatus.Departed)
                return false;

            trip.Status = TripStatus.Arrived;
            trip.UpdatedAt = DateTime.UtcNow;
            await _tripRepository.UpdateAsync(trip);
            return true;
        }

        public async Task<bool> CancelTripWithCascadeAsync(int id, int actorUserId, string? reason)
        {
            var trip = await _db.Trip.FirstOrDefaultAsync(t => t.Id == id);
            if (trip == null) return false;
            if (trip.Status == TripStatus.Cancelled) return false;
            if (trip.Status == TripStatus.Arrived) return false;

            // Pull the affected bookings + tickets + payments in one go.
            var bookingPassengers = await _db.BookingPassengers
                .Include(bp => bp.Booking).ThenInclude(b => b.Payments)
                .Include(bp => bp.Ticket)
                .Where(bp => bp.TripId == id)
                .ToListAsync();

            var bookingIds = bookingPassengers
                .Select(bp => bp.BookingId)
                .Distinct()
                .ToList();

            var bookings = bookingPassengers
                .Select(bp => bp.Booking)
                .Where(b => b != null)
                .GroupBy(b => b.Id)
                .Select(g => g.First())
                .ToList();

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                trip.Status = TripStatus.Cancelled;
                trip.UpdatedAt = DateTime.UtcNow;

                var now = DateTime.UtcNow;
                int? actor = actorUserId > 0 ? actorUserId : (int?)null;
                var notifiedBookingIds = new List<int>();

                foreach (var booking in bookings)
                {
                    if (booking.Status == BookingStatus.Cancelled)
                        continue;

                    booking.Status = BookingStatus.Cancelled;
                    booking.CancelledAt = now;
                    booking.CancelledBy = actor;
                    booking.CancellationReason = reason ?? "Trip cancelled";

                    var completedPayment = booking.Payments
                        .FirstOrDefault(p => p.Status == PaymentStatus.Completed);

                    if (completedPayment != null)
                    {
                        booking.RefundAmount = completedPayment.Amount;
                        _db.Refunds.Add(new Refund
                        {
                            BookingId = booking.Id,
                            PaymentId = completedPayment.Id,
                            RefundNumber = $"RF-{booking.Reference}-{now.Ticks % 100000}",
                            Amount = completedPayment.Amount,
                            Currency = completedPayment.Currency ?? "SDG",
                            Status = RefundStatus.Pending,
                            Method = RefundMethod.Original,
                            Reason = reason ?? "Trip cancelled",
                            CreatedAt = now,
                        });
                    }

                    if (booking.UserId.HasValue)
                        notifiedBookingIds.Add(booking.Id);
                }

                foreach (var bp in bookingPassengers)
                {
                    if (bp.Ticket != null && bp.Ticket.Status != TicketStatus.Cancelled)
                        bp.Ticket.Status = TicketStatus.Cancelled;
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                foreach (var bookingId in notifiedBookingIds)
                    await _bookingNotifications.NotifyTripCancelledAsync(bookingId, reason);

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> HasOverlappingTripsAsync(int trainId, DateTime departureTime, DateTime arrivalTime, int? excludeTripId = null)
        {
            var query = _tripRepository.GetTableNoTracking()
                .Where(t => t.TrainId == trainId && t.Status != TripStatus.Cancelled)
                .Where(t => (t.DepartureTime < arrivalTime && t.ArrivalTime > departureTime));

            if (excludeTripId.HasValue)
                query = query.Where(t => t.Id != excludeTripId.Value);

            return await query.AnyAsync();
        }

        public async Task InitializeTripSeatsAsync(int tripId, int trainId)
        {
            var seats = await _coachRepository.GetTableNoTracking()
                .Where(c => c.TrainId == trainId)
                .SelectMany(c => c.Seats)
                .ToListAsync();

            var tripSeats = seats.Select(seat => new TripSeat
            {
                TripId = tripId,
                SeatId = seat.Id,
                Status = SeatStatus.Available
            }).ToList();

            await _tripSeatRepository.AddRangeAsync(tripSeats);
        }

        public async Task<SegmentSeatsDto?> GetSegmentSeatsAsync(int tripId, int boardingStationId, int alightingStationId)
        {
            // Manual split — fetch the four pieces this method needs in
            // separate queries instead of one giant cartesian join through
            // Train×Coaches×Seats × Route×RouteStations × TripSeats. That
            // join is what caused the ~60s response time on busy trips.
            //
            // We intentionally do NOT use EF's `.AsSplitQuery()` because it
            // combines poorly with `.AsNoTracking()` (NRE in production when
            // EF can't stitch the sub-results back together). Plain manual
            // queries are predictable.
            var trip = await _tripRepository.GetTableNoTracking()
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.Route).ThenInclude(r => r.RouteStations)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null) return null;

            // Train → coaches → seats (filtered to this trip's train).
            var coaches = await _coachRepository.GetTableNoTracking()
                .Where(c => c.TrainId == trip.TrainId)
                .Include(c => c.Seats)
                .OrderBy(c => c.Sequence)
                .ToListAsync();

            // TripSeats for this trip — needed for per-seat status + tripSeatId.
            var tripSeats = await _tripSeatRepository.GetTableNoTracking()
                .Where(ts => ts.TripId == trip.Id)
                .ToListAsync();

            // Build a stationId → stopOrder dictionary once. Used to be a
            // linear scan over RouteStations called per booking, which is
            // O(bookings × stops) — the second big cost in this method.
            var maxIntermediateOrder = trip.Route.RouteStations.Count == 0
                ? 0
                : trip.Route.RouteStations.Max(rs => rs.StopOrder);
            var stopOrderByStation = new Dictionary<int, int>
            {
                [trip.Route.OriginStationId] = 0,
            };
            foreach (var rs in trip.Route.RouteStations)
                stopOrderByStation[rs.StationId] = rs.StopOrder;
            stopOrderByStation[trip.Route.DestinationStationId] = maxIntermediateOrder + 1;

            if (!stopOrderByStation.TryGetValue(boardingStationId, out var boardingOrder) ||
                !stopOrderByStation.TryGetValue(alightingStationId, out var alightingOrder) ||
                alightingOrder <= boardingOrder)
                return null;

            // Existing bookings on this trip that aren't cancelled.
            var existing = await _tripRepository.GetTableNoTracking()
                .Where(t => t.Id == tripId)
                .SelectMany(t => t.BookingPassengers)
                .Where(bp => bp.Booking.Status != BookingStatus.Cancelled && bp.TripSeatId != null)
                .Select(bp => new { bp.TripSeatId, bp.BoardingStationId, bp.AlightingStationId })
                .ToListAsync();

            // Pre-compute stop orders for every booking — O(1) dictionary
            // lookups instead of the previous O(stops) linear scan per row.
            var existingByTripSeat = existing
                .Select(bp => new
                {
                    bp.TripSeatId,
                    BOrder = stopOrderByStation.TryGetValue(bp.BoardingStationId, out var bo) ? (int?)bo : null,
                    AOrder = stopOrderByStation.TryGetValue(bp.AlightingStationId, out var ao) ? (int?)ao : null,
                })
                .Where(x => x.BOrder.HasValue && x.AOrder.HasValue)
                .GroupBy(x => x.TripSeatId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => (B: x.BOrder!.Value, A: x.AOrder!.Value)).ToList());

            var tripSeatBySeatId = tripSeats.ToDictionary(ts => ts.SeatId, ts => ts);

            var coachDtos = new List<CoachSeatsDto>();
            int totalSeats = 0;
            int availableCount = 0;

            foreach (var coach in coaches)
            {
                var coachDto = new CoachSeatsDto
                {
                    Id = coach.Id,
                    CoachNumber = coach.CoachNumber,
                    Class = coach.Class.ToString(),
                    Seats = new List<AvailableSeatDto>(),
                };

                foreach (var seat in coach.Seats.OrderBy(s => s.SeatNumber))
                {
                    totalSeats++;
                    if (!tripSeatBySeatId.TryGetValue(seat.Id, out var tripSeat))
                    {
                        // No TripSeat row for this seat — treat as unavailable.
                        coachDto.Seats.Add(new AvailableSeatDto
                        {
                            Id = seat.Id,
                            TripSeatId = 0,
                            SeatNumber = seat.SeatNumber,
                            IsWindow = seat.IsWindow,
                            IsAccessible = seat.IsAccessible,
                            IsAvailable = false,
                        });
                        continue;
                    }

                    bool available = tripSeat.Status != SeatStatus.Maintenance;
                    if (available && existingByTripSeat.TryGetValue(tripSeat.Id, out var ranges))
                    {
                        // Overlap check: any existing booking [b2,a2] overlaps requested [b,a]?
                        // Ranges overlap iff b < a2 && b2 < a.
                        foreach (var (b2, a2) in ranges)
                        {
                            if (boardingOrder < a2 && b2 < alightingOrder)
                            {
                                available = false;
                                break;
                            }
                        }
                    }

                    if (available) availableCount++;
                    coachDto.Seats.Add(new AvailableSeatDto
                    {
                        Id = seat.Id,
                        TripSeatId = tripSeat.Id,
                        SeatNumber = seat.SeatNumber,
                        IsWindow = seat.IsWindow,
                        IsAccessible = seat.IsAccessible,
                        IsAvailable = available,
                    });
                }

                coachDtos.Add(coachDto);
            }

            return new SegmentSeatsDto
            {
                TripId = trip.Id,
                BoardingStationId = boardingStationId,
                AlightingStationId = alightingStationId,
                BoardingStationName = StationNameAt(trip.Route, boardingStationId),
                AlightingStationName = StationNameAt(trip.Route, alightingStationId),
                TotalSeats = totalSeats,
                AvailableCount = availableCount,
                Coaches = coachDtos,
            };
        }

        /// <summary>
        /// Stop position of <paramref name="stationId"/> on a route. Origin is 0,
        /// destination is one past the highest intermediate StopOrder, intermediates
        /// use their RouteStation.StopOrder, anything else is null.
        /// </summary>
        public static int? StopOrderOnRoute(Data.Entity.Route route, int stationId)
        {
            if (route.OriginStationId == stationId) return 0;
            var intermediate = route.RouteStations.FirstOrDefault(rs => rs.StationId == stationId);
            if (intermediate != null) return intermediate.StopOrder;
            if (route.DestinationStationId == stationId)
            {
                var maxIntermediate = route.RouteStations.Any()
                    ? route.RouteStations.Max(rs => rs.StopOrder)
                    : 0;
                return maxIntermediate + 1;
            }
            return null;
        }

        private static string StationNameAt(Data.Entity.Route route, int stationId)
        {
            if (route.OriginStationId == stationId)
                return route.OriginStation.NameAr ?? route.OriginStation.NameEn ?? "";
            if (route.DestinationStationId == stationId)
                return route.DestinationStation.NameAr ?? route.DestinationStation.NameEn ?? "";
            var rs = route.RouteStations.FirstOrDefault(x => x.StationId == stationId);
            return rs == null ? "" : (rs.Station.NameAr ?? rs.Station.NameEn ?? "");
        }
    }
}

