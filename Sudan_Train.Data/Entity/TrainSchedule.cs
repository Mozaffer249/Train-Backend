using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Commons;

namespace Sudan_Train.Data.Entity
{
    public class TrainSchedule : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        public int TrainId { get; set; }

        [ForeignKey(nameof(TrainId))]
        public Train Train { get; set; } = default!;

        public int RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = default!;

        public RecurrenceType RecurrenceType { get; set; }

        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }

        /// <summary>
        /// JSON array of day numbers (e.g., "[1,3,5]" for Monday, Wednesday, Friday)
        /// </summary>
        [MaxLength(100)]
        public string? DaysOfWeek { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
