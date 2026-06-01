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
        public decimal? DiscountPercent { get; set; }
        public string Currency { get; set; } = default!;
        public decimal FinalPrice { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Populated by GetApplicableFare so the customer/admin UI can render a
        // transparent line-by-line receipt.
        public FareBreakdownDto? Breakdown { get; set; }
    }

    // base → discount → total. No VAT.
    public class FareBreakdownDto
    {
        public decimal BasePrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; } = "SDG";
    }
}
