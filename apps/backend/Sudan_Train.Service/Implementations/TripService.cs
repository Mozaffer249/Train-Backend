using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
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

        public TripService(
            ITripRepository tripRepository,
            ITripSeatRepository tripSeatRepository,
            ITrainRepository trainRepository,
            IRouteRepository routeRepository,
            ICoachRepository coachRepository)
        {
            _tripRepository = tripRepository;
            _tripSeatRepository = tripSeatRepository;
            _trainRepository = trainRepository;
            _routeRepository = routeRepository;
            _coachRepository = coachRepository;
        }

        public async Task<TripDto> CreateTripAsync(int trainId, int routeId, DateTime departureTime, DateTime arrivalTime)
        {
            var trip = new Trip
            {
                TrainId = trainId,
                RouteId = routeId,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Status = "Scheduled",
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
                Status = tripDetails.Status,
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
                Status = trip.Status,
                TotalSeats = trip.TripSeats.Count,
                AvailableSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Available),
                BookedSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Occupied)
            };
        }

        public async Task<List<TripDto>> GetAllTripsAsync(DateTime? date = null, int? routeId = null, string? status = null)
        {
            var query = _tripRepository.GetTableNoTracking()
                .Include(t => t.Train)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.TripSeats)
                .AsQueryable();

            if (date.HasValue)
            {
                var startOfDay = date.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(t => t.DepartureTime >= startOfDay && t.DepartureTime < endOfDay);
            }

            if (routeId.HasValue)
                query = query.Where(t => t.RouteId == routeId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            var trips = await query
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

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
                Status = t.Status,
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
            trip.Status = status;
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
                Status = trip.Status,
                TotalSeats = trip.TripSeats.Count,
                AvailableSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Available),
                BookedSeats = trip.TripSeats.Count(ts => ts.Status == SeatStatus.Occupied)
            };
        }

        public async Task<bool> CancelTripAsync(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null)
                return false;

            if (trip.Status == "Cancelled")
                return false;

            if (trip.Status == "Completed")
                return false;

            trip.Status = "Cancelled";
            trip.UpdatedAt = DateTime.UtcNow;

            await _tripRepository.UpdateAsync(trip);
            return true;
        }

        public async Task<bool> HasOverlappingTripsAsync(int trainId, DateTime departureTime, DateTime arrivalTime, int? excludeTripId = null)
        {
            var query = _tripRepository.GetTableNoTracking()
                .Where(t => t.TrainId == trainId && t.Status != "Cancelled")
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
            var trip = await _tripRepository.GetTableNoTracking()
                .Include(t => t.Train)
                    .ThenInclude(tr => tr.Coaches)
                        .ThenInclude(c => c.Seats)
                .Include(t => t.Route).ThenInclude(r => r.OriginStation)
                .Include(t => t.Route).ThenInclude(r => r.DestinationStation)
                .Include(t => t.Route).ThenInclude(r => r.RouteStations).ThenInclude(rs => rs.Station)
                .Include(t => t.TripSeats)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null) return null;

            var boarding = StopOrderOnRoute(trip.Route, boardingStationId);
            var alighting = StopOrderOnRoute(trip.Route, alightingStationId);
            if (boarding == null || alighting == null || alighting.Value <= boarding.Value)
                return null;

            // Existing bookings on this trip that aren't cancelled.
            var existing = await _tripRepository.GetTableNoTracking()
                .Where(t => t.Id == tripId)
                .SelectMany(t => t.BookingPassengers)
                .Where(bp => bp.Booking.Status != BookingStatus.Cancelled && bp.TripSeatId != null)
                .Select(bp => new { bp.TripSeatId, bp.BoardingStationId, bp.AlightingStationId })
                .ToListAsync();

            // Pre-compute stop orders for every booking so the overlap check is O(1) per seat.
            var existingByTripSeat = existing
                .Select(bp => new
                {
                    bp.TripSeatId,
                    BOrder = StopOrderOnRoute(trip.Route, bp.BoardingStationId),
                    AOrder = StopOrderOnRoute(trip.Route, bp.AlightingStationId),
                })
                .Where(x => x.BOrder.HasValue && x.AOrder.HasValue)
                .GroupBy(x => x.TripSeatId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => (B: x.BOrder!.Value, A: x.AOrder!.Value)).ToList());

            var tripSeatBySeatId = trip.TripSeats.ToDictionary(ts => ts.SeatId, ts => ts);

            var coachDtos = new List<CoachSeatsDto>();
            int totalSeats = 0;
            int availableCount = 0;

            foreach (var coach in trip.Train.Coaches.OrderBy(c => c.Sequence))
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
                            if (boarding.Value < a2 && b2 < alighting.Value)
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

