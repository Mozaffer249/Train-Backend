using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Commons;

namespace Sudan_Train.Data.Entity
{
    public class Trip : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int TrainId { get; set; }

        [ForeignKey(nameof(TrainId))]
        public Train Train { get; set; } = default!;

        public int RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = default!;

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Scheduled";

        public ICollection<TripSeat> TripSeats { get; set; } = new List<TripSeat>();
        public ICollection<BookingPassenger> BookingPassengers { get; set; } = new List<BookingPassenger>();
    }
}

