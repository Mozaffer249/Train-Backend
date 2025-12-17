using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Commons;

namespace Sudan_Train.Data.Entity
{
    public class Route : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? NameEn { get; set; }

        [MaxLength(200)]
        public string? NameAr { get; set; }

        public int OriginStationId { get; set; }

        [ForeignKey(nameof(OriginStationId))]
        public Station OriginStation { get; set; } = default!;

        public int DestinationStationId { get; set; }

        [ForeignKey(nameof(DestinationStationId))]
        public Station DestinationStation { get; set; } = default!;

        public decimal? DistanceKm { get; set; }

        public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();
    }
}

