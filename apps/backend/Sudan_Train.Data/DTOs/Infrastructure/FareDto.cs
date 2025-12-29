namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class FareDto
    {
        public int Id { get; set; }
        public int? RouteId { get; set; }
        public int? OriginStationId { get; set; }
        public int? DestinationStationId { get; set; }
        public int? TripId { get; set; }
        public string CoachClass { get; set; } = default!;
        public decimal BasePrice { get; set; }
        public decimal? PricePerKm { get; set; }
        public decimal VatRate { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string Currency { get; set; } = default!;
        public decimal FinalPrice { get; set; }
        public decimal TotalWithVat { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
