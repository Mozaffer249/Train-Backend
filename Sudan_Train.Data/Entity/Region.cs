using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sudan_Train.Data.Entity
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = default!;

        [Required, MaxLength(100)]
        public string NameAr { get; set; } = default!;

        [Required, MaxLength(20)]
        public string Code { get; set; } = default!;

        public ICollection<State> States { get; set; } = new List<State>();
    }
}
