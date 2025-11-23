using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Trip
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

        public string Status { get; set; } = "Scheduled";

        public ICollection<TripSeat> TripSeats { get; set; } = new List<TripSeat>();
    }
}

