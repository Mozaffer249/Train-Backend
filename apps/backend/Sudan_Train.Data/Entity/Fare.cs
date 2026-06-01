using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Fare
    {
        [Key]
        public int Id { get; set; }

        // Route-based pricing
        public int? RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route? Route { get; set; }

        // Origin and destination for segment pricing
        public int? OriginStationId { get; set; }

        [ForeignKey(nameof(OriginStationId))]
        public Station? OriginStation { get; set; }

        public int? DestinationStationId { get; set; }

        [ForeignKey(nameof(DestinationStationId))]
        public Station? DestinationStation { get; set; }

        // Trip-specific override pricing
        public int? TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip? Trip { get; set; }

        public CoachClass CoachClass { get; set; }

        public decimal BasePrice { get; set; }

        public decimal? DiscountPercent { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "SDG";

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;

        public DateTime? EffectiveTo { get; set; }

        // The customer pays base − discount. No VAT line.
        [NotMapped]
        public decimal FinalPrice => BasePrice - (BasePrice * (DiscountPercent ?? 0m) / 100m);
    }
}


