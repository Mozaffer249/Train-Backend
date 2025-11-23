using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Fare
    {
        [Key]
        public int Id { get; set; }

        public int? TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip? Trip { get; set; }

        public CoachClass CoachClass { get; set; }

        public decimal Price { get; set; }

        public decimal VatRate { get; set; }

        public decimal? DiscountPercent { get; set; }

        public string Currency { get; set; } = "SDG";

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}

