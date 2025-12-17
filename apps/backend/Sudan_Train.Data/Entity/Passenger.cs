using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Sudan_Train.Data.Commons;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Data.Entity
{
    public class Passenger : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int? CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }

        [Required, MaxLength(200)]
        public string FullNameEn { get; set; } = default!;

        [MaxLength(200)]
        public string? FullNameAr { get; set; }

        [Required, MaxLength(50)]
        [EncryptColumn]
        public string IdNumber { get; set; } = default!;

        [MaxLength(20)]
        public string? Gender { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }
    }
}