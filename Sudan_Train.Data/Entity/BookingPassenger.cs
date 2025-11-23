using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class BookingPassenger
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = default!;

        public int PassengerId { get; set; }

        [ForeignKey(nameof(PassengerId))]
        public Passenger Passenger { get; set; } = default!;

        public int TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } = default!;

        public int? TripSeatId { get; set; }

        [ForeignKey(nameof(TripSeatId))]
        public TripSeat? TripSeat { get; set; }

        public int? FareId { get; set; }

        [ForeignKey(nameof(FareId))]
        public Fare? Fare { get; set; }

        public decimal Price { get; set; }
        public string? SeatNumber { get; set; }

        public Ticket? Ticket { get; set; }
    }
}