using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class TripSeat
    {
        [Key]
        public int Id { get; set; }

        public int TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } = default!;

        public int SeatId { get; set; }

        [ForeignKey(nameof(SeatId))]
        public Seat Seat { get; set; } = default!;

        public int CoachId { get; set; }

        [ForeignKey(nameof(CoachId))]
        public Coach Coach { get; set; } = default!;

        public SeatStatus Status { get; set; } = SeatStatus.Available;

        public decimal Price { get; set; }
    }
}

