using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class RouteStation
    {
        [Key]
        public int Id { get; set; }

        public int RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = default!;

        public int StationId { get; set; }

        [ForeignKey(nameof(StationId))]
        public Station Station { get; set; } = default!;

        public int StopOrder { get; set; }

        public TimeSpan? ArrivalOffset { get; set; }

        public TimeSpan? DepartureOffset { get; set; }
    }
}

