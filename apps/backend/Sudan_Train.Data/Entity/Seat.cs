using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        public int CoachId { get; set; }

        [ForeignKey(nameof(CoachId))]
        public Coach Coach { get; set; } = default!;

        public string SeatNumber { get; set; } = default!;

        public bool IsWindow { get; set; }

        public bool IsAccessible { get; set; }

        public ICollection<TripSeat> TripSeats { get; set; } = new List<TripSeat>();
    }
}

