using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = default!;

        [Required, MaxLength(100)]
        public string NameAr { get; set; } = default!;

        public int StateId { get; set; }

        [ForeignKey(nameof(StateId))]
        public State State { get; set; } = default!;

        public ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}

