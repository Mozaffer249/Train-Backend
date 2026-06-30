using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Service.Abstracts
{
    public interface ISeatHoldService
    {
        Task<HoldSeatsResult> HoldSeatsAsync(
            int userId,
            int tripId,
            int boardingStationId,
            int alightingStationId,
            IReadOnlyList<int> seatIds,
            Guid? holdGroupId = null);

        Task ReleaseHoldsAsync(int userId, Guid? holdGroupId = null);

        Task<(bool Valid, string? Error)> ValidateHoldsAsync(
            int userId,
            int tripId,
            int boardingStationId,
            int alightingStationId,
            IReadOnlyList<int> seatIds);

        Task DeleteHoldsForSeatsAsync(int userId, int tripId, IReadOnlyList<int> seatIds);

        Task<List<ActiveSeatHold>> GetActiveHoldsForTripAsync(int tripId, int? excludeUserId = null);
    }

    public class ActiveSeatHold
    {
        public int UserId { get; set; }
        public int TripSeatId { get; set; }
        public int SeatId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
    }
}
