namespace Sudan_Train.Data.DTOs.Booking
{
    public class SeatHoldResultDto
    {
        public Guid HoldGroupId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<int> HeldSeatIds { get; set; } = new();
    }

    public class HoldSeatsResult
    {
        public bool Success { get; set; }
        public bool Conflict { get; set; }
        public string? Error { get; set; }
        public SeatHoldResultDto? Data { get; set; }
    }
}
