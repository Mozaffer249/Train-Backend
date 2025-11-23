using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sudan_Train.Data.Entity
{
    public class Train
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string TrainNumber { get; set; } = default!;

        public string? NameEn { get; set; }

        public string? NameAr { get; set; }

        public CoachClass Type { get; set; }

        public ICollection<Coach> Coaches { get; set; } = new List<Coach>();
    }
}

