using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sudan_Train.Data.Entity
{
    public class State
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = default!;

        [Required, MaxLength(100)]
        public string NameAr { get; set; } = default!;

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}