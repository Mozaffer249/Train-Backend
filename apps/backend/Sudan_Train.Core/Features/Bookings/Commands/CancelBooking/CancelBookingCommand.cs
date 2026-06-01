using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommand : IRequest<Response<string>>
    {
        public int BookingId { get; set; }
        public string? Reason { get; set; }
    }
}
