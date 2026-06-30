using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    /// <summary>
    /// Temporary seat reservation while a user completes booking (5-minute TTL).
    /// </summary>
    public class SeatHold
    {
        [Key]
        public int Id { get; set; }

        public Guid HoldGroupId { get; set; }

        public int UserId { get; set; }

        public int TripId { get; set; }

        public int TripSeatId { get; set; }

        [ForeignKey(nameof(TripSeatId))]
        public TripSeat TripSeat { get; set; } = default!;

        public int BoardingStationId { get; set; }

        public int AlightingStationId { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
