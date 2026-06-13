namespace Sudan_Train.Data.DTOs.Booking
{
    // Manifest = the staff-facing view of who is supposed to be on a given
    // trip. Used to render the boarding screen.
    public class TripManifestDto
    {
        public int TripId { get; set; }
        public string? TrainNumber { get; set; }
        public string? RouteNameEn { get; set; }
        public string? RouteNameAr { get; set; }
        public string? OriginStationEn { get; set; }
        public string? OriginStationAr { get; set; }
        public string? DestinationStationEn { get; set; }
        public string? DestinationStationAr { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = "Scheduled";

        // Counts so the UI can show a quick summary.
        public int TotalTickets { get; set; }
        public int BoardedCount { get; set; }
        public int IssuedCount { get; set; }
        public int NoShowCount { get; set; }
        public int CancelledCount { get; set; }

        public List<ManifestRowDto> Rows { get; set; } = new();
    }

    public class ManifestRowDto
    {
        public int TicketId { get; set; }
        public string? TicketNumber { get; set; }
        public int BookingId { get; set; }
        public string? BookingReference { get; set; }

        public string? PassengerNameEn { get; set; }
        public string? PassengerNameAr { get; set; }
        public string? IdNumber { get; set; }

        public string? SeatNumber { get; set; }
        public string? CoachNumber { get; set; }
        public string? CoachClass { get; set; }

        public int BoardingStationId { get; set; }
        public string? BoardingStationEn { get; set; }
        public string? BoardingStationAr { get; set; }
        public int AlightingStationId { get; set; }
        public string? AlightingStationEn { get; set; }
        public string? AlightingStationAr { get; set; }

        public string Status { get; set; } = "Issued";
        public DateTime? BoardedAt { get; set; }
        public int? BoardedByUserId { get; set; }
    }
}
