using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Service.Abstracts
{
    public class CreateBookingInput
    {
        public int? UserId { get; set; }
        /// <summary>Authenticated user who placed seat holds (JWT user; may differ from UserId for counter sales).</summary>
        public int? HoldingUserId { get; set; }
        public int TripId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
        // One entry per ticket. Each pairs a seat (on this trip) with the
        // passenger riding it. Backend handles N≥1 atomically — total = sum
        // of per-seat fares, payment is one mock charge for the sum.
        public List<PassengerSeatInput> Passengers { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }
        public string? CardLast4 { get; set; }
    }

    public class PassengerSeatInput
    {
        public int SeatId { get; set; }
        // The CoachClass of this seat — used for fare resolution. Customer
        // sends what they saw in the seat grid; backend re-verifies.
        public CoachClass CoachClass { get; set; }
        public PassengerInput Passenger { get; set; } = default!;
    }

    public class PassengerInput
    {
        public string FullNameEn { get; set; } = default!;
        public string? FullNameAr { get; set; }
        public string IdNumber { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    public class BookingListParams
    {
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class BookingCreationResult
    {
        public BookingDto? Booking { get; set; }
        public string? Error { get; set; }
        public bool Conflict { get; set; }   // 409 — seat taken between read and write
        public bool NotFound { get; set; }   // 404 — trip/seat/station missing
        public bool Invalid { get; set; }    // 400 — bad request (seg ordering, no fare)
    }

    public interface IBookingService
    {
        Task<BookingCreationResult> CreateBookingAsync(CreateBookingInput input);
        Task<bool> CancelBookingAsync(int bookingId, int? userId, bool isAdmin, string? reason);
        Task<BookingDto?> GetByIdAsync(int bookingId, int? userId, bool isAdmin);
        Task<List<BookingDto>> GetMineAsync(int userId);
        Task<List<BookingDto>> GetAllAsync(BookingListParams query);
    }
}
