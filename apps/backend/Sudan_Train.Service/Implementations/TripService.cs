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
                TrainName = tripDetails.Train.NameEn ?? "",
                RouteId = tripDetails.RouteId,
                RouteName = tripDetails.Route.NameEn ?? "",
                OriginStation = tripDetails.Route.OriginStation.NameEn,
                DestinationStation = tripDetails.Route.DestinationStation.NameEn,
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
                TrainName = trip.Train.NameEn ?? "",
                RouteId = trip.RouteId,
                RouteName = trip.Route.NameEn ?? "",
                OriginStation = trip.Route.OriginStation.NameEn,
                DestinationStation = trip.Route.DestinationStation.NameEn,
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
                TrainName = t.Train.NameEn ?? "",
                RouteId = t.RouteId,
                RouteName = t.Route.NameEn ?? "",
                OriginStation = t.Route.OriginStation.NameEn,
                DestinationStation = t.Route.DestinationStation.NameEn,
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
                TrainName = trip.Train.NameEn ?? "",
                RouteId = trip.RouteId,
                RouteName = trip.Route.NameEn ?? "",
                OriginStation = trip.Route.OriginStation.NameEn,
                DestinationStation = trip.Route.DestinationStation.NameEn,
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
    }
}

