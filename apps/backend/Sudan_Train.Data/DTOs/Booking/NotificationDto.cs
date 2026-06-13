namespace Sudan_Train.Data.DTOs.Booking
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int? BookingId { get; set; }
        public string? BookingReference { get; set; }
        public string Type { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Message { get; set; } = default!;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
