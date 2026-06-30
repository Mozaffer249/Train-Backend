using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Service.Abstracts
{
    public interface ITripService
    {
        Task<TripDto> CreateTripAsync(int trainId, int routeId, DateTime departureTime, DateTime arrivalTime);
        Task<TripDto?> GetTripByIdAsync(int id);
        // assignedStationIds: when non-null, filter to trips whose route
        // touches any station in the set (origin, destination, or intermediate
        // RouteStation). Null = no station filter (admin / anonymous / customer).
        // upcomingOnly: when true, exclude trips whose DepartureTime is in
        // the past (server clock).
        Task<List<TripDto>> GetAllTripsAsync(
            DateTime? date = null,
            int? routeId = null,
            string? status = null,
            List<int>? assignedStationIds = null,
            bool upcomingOnly = false);
        Task<TripDto> UpdateTripAsync(int id, DateTime departureTime, DateTime arrivalTime, string status);
        Task<bool> CancelTripAsync(int id);

        // Operational transitions used by the boarding/dispatch flow.
        Task<bool> MarkDepartedAsync(int id);
        Task<bool> MarkArrivedAsync(int id);

        // Cancel a trip and cascade: flips all active bookings + tickets to
        // Cancelled, inserts a Refund row per booking with a completed
        // payment, inserts one in-app Notification per affected user.
        Task<bool> CancelTripWithCascadeAsync(int id, int actorUserId, string? reason);

        Task<bool> HasOverlappingTripsAsync(int trainId, DateTime departureTime, DateTime arrivalTime, int? excludeTripId = null);
        Task InitializeTripSeatsAsync(int tripId, int trainId);

        // Per-segment seat availability: returns the seat grid annotated with
        // IsAvailable computed from existing BookingPassenger overlaps for the
        // requested boarding→alighting segment.
        Task<SegmentSeatsDto?> GetSegmentSeatsAsync(int tripId, int boardingStationId, int alightingStationId, int? currentUserId = null);
    }
}

