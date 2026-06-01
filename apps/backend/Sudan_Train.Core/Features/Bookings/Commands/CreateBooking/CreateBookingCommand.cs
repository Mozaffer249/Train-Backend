using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommand : IRequest<Response<BookingDto>>
    {
        public int TripId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;
        public string? CardLast4 { get; set; }

        // One entry per ticket / passenger. Must have at least one.
        public List<PassengerSeatInputDto> Passengers { get; set; } = new();
    }

    public class PassengerSeatInputDto
    {
        public int SeatId { get; set; }
        public CoachClass CoachClass { get; set; } = CoachClass.Second;
        public PassengerInputDto Passenger { get; set; } = default!;
    }

    public class PassengerInputDto
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
}
