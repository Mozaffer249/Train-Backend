namespace Sudan_Train.Data.DTOs.Booking
{
    public class RefundDto
    {
        public int Id { get; set; }
        public string RefundNumber { get; set; } = default!;
        public int BookingId { get; set; }
        public string? BookingReference { get; set; }
        public int? UserId { get; set; }
        public string? UserFullName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "SDG";
        public string Status { get; set; } = "Pending";
        public string Method { get; set; } = "Original";
        public string? Reason { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
