namespace Sudan_Train.Data.DTOs.Payment
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string BookingRef { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string Method { get; set; } = default!;
        public string Status { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public string? CardBrand { get; set; }
        public string? CardLast4 { get; set; }
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentStatusCountDto
    {
        public string Status { get; set; } = default!;
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentMethodCountDto
    {
        public string Method { get; set; } = default!;
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentsSummaryDto
    {
        public decimal TotalCollected { get; set; }
        public int Count { get; set; }
        public List<PaymentStatusCountDto> ByStatus { get; set; } = new();
        public List<PaymentMethodCountDto> ByMethod { get; set; } = new();
    }

    public class PaymentsReportDto
    {
        public List<PaymentDto> Items { get; set; } = new();
        public PaymentsSummaryDto Summary { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
