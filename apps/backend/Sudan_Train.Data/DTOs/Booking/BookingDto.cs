using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Data.DTOs.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string BookingRef { get; set; } = default!;
        public int TripId { get; set; }
        public string TrainName { get; set; } = default!;
        public string RouteName { get; set; } = default!;

        // Segment the booking covers — same for every passenger on the booking.
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
        public string BoardingStationName { get; set; } = default!;
        public string AlightingStationName { get; set; } = default!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        // Convenience: primary (first) passenger's seat + class. Kept so legacy
        // single-passenger callers don't break. `Passengers` is the source of truth
        // when more than one seat was booked.
        public string CoachClass { get; set; } = default!;
        public string SeatNumber { get; set; } = default!;
        public BookingPassengerInfoDto Passenger { get; set; } = default!;
        public TicketInfoDto? Ticket { get; set; }

        // ALL passengers on this booking. For solo bookings the list has 1 entry;
        // multi-seat bookings have N.
        public List<BookingPassengerDetailDto> Passengers { get; set; } = new();

        public decimal BasePrice { get; set; }
        // Total across all passengers on the booking. Equals sum of Passengers[i].Price.
        public decimal Total { get; set; }
        public string Currency { get; set; } = "SDG";

        // Frozen at booking time. For multi-seat bookings this is the per-seat
        // breakdown (price walk for one ticket — each seat costs this much).
        public FareBreakdownDto? Breakdown { get; set; }

        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

    public class BookingPassengerInfoDto
    {
        public string FullNameEn { get; set; } = default!;
        public string? FullNameAr { get; set; }
        public string IdNumber { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
    }

    // One entry per booked seat — what step 4 / the e-ticket modal iterates over.
    public class BookingPassengerDetailDto
    {
        public BookingPassengerInfoDto Passenger { get; set; } = default!;
        public string SeatNumber { get; set; } = default!;
        public string CoachClass { get; set; } = default!;
        public decimal Price { get; set; }
        public TicketInfoDto? Ticket { get; set; }
    }

    public class TicketInfoDto
    {
        public string TicketNumber { get; set; } = default!;
        public string? QrPayload { get; set; }
        public string Status { get; set; } = default!;
    }
}
