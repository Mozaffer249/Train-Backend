using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Data.Entity
{
    public class Passenger
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int? CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }

        public string FullNameEn { get; set; } = default!;
        public string? FullNameAr { get; set; }

        public string IdNumber { get; set; } = default!;
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
    }
}