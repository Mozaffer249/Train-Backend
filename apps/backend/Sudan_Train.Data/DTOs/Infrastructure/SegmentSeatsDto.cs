namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class SegmentSeatsDto
    {
        public int TripId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
        public string BoardingStationName { get; set; } = default!;
        public string AlightingStationName { get; set; } = default!;
        public int TotalSeats { get; set; }
        public int AvailableCount { get; set; }
        public List<CoachSeatsDto> Coaches { get; set; } = new();
    }

    public class CoachSeatsDto
    {
        public int Id { get; set; }
        public string CoachNumber { get; set; } = default!;
        public string Class { get; set; } = default!;
        public List<AvailableSeatDto> Seats { get; set; } = new();
    }

    public class AvailableSeatDto
    {
        public int Id { get; set; }
        public int TripSeatId { get; set; }
        public string SeatNumber { get; set; } = default!;
        public bool IsWindow { get; set; }
        public bool IsAccessible { get; set; }
        public bool IsAvailable { get; set; }
    }
}
