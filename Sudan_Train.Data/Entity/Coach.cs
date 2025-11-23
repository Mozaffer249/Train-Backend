using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Coach
    {
        [Key]
        public int Id { get; set; }

        public int TrainId { get; set; }

        [ForeignKey(nameof(TrainId))]
        public Train Train { get; set; } = default!;

        public string CoachNumber { get; set; } = default!;

        public CoachClass Class { get; set; }

        public int Capacity { get; set; }

        public int Sequence { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}

