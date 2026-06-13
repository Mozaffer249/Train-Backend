using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Bookings.Commands.CreateBooking;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Bookings.Commands.CreateCounterBooking
{
    // Counter booking — Staff sells tickets on behalf of a customer. The
    // booking is hung on CustomerUserId when present (registered customer),
    // or UserId = null for walk-ins. Same per-segment invariants apply.
    public class CreateCounterBookingCommand : IRequest<Response<BookingDto>>
    {
        public int? CustomerUserId { get; set; }

        public int TripId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }

        // Defaults to Cash; admin/staff can override per booking.
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? CardLast4 { get; set; }

        public List<PassengerSeatInputDto> Passengers { get; set; } = new();
    }
}
