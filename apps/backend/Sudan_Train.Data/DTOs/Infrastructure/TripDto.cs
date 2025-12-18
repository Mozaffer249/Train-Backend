namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class TripDto
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public string TrainNumber { get; set; } = default!;
        public string TrainName { get; set; } = default!;
        public int RouteId { get; set; }
        public string RouteName { get; set; } = default!;
        public string OriginStation { get; set; } = default!;
        public string DestinationStation { get; set; } = default!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = default!;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int BookedSeats { get; set; }
    }
}

