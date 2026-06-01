using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Service.Abstracts
{
    public interface ITripService
    {
        Task<TripDto> CreateTripAsync(int trainId, int routeId, DateTime departureTime, DateTime arrivalTime);
        Task<TripDto?> GetTripByIdAsync(int id);
        Task<List<TripDto>> GetAllTripsAsync(DateTime? date = null, int? routeId = null, string? status = null);
        Task<TripDto> UpdateTripAsync(int id, DateTime departureTime, DateTime arrivalTime, string status);
        Task<bool> CancelTripAsync(int id);
        Task<bool> HasOverlappingTripsAsync(int trainId, DateTime departureTime, DateTime arrivalTime, int? excludeTripId = null);
        Task InitializeTripSeatsAsync(int tripId, int trainId);

        // Per-segment seat availability: returns the seat grid annotated with
        // IsAvailable computed from existing BookingPassenger overlaps for the
        // requested boarding→alighting segment.
        Task<SegmentSeatsDto?> GetSegmentSeatsAsync(int tripId, int boardingStationId, int alightingStationId);
    }
}

