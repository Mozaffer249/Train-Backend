using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sudan_Train.Data.Commons;

namespace Sudan_Train.Data.Entity
{
    public class Train : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string TrainNumber { get; set; } = default!;

        [Required, MaxLength(200)]
        public string? NameEn { get; set; }

        [MaxLength(200)]
        public string? NameAr { get; set; }

        public ICollection<Coach> Coaches { get; set; } = new List<Coach>();
        public ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();
    }
}