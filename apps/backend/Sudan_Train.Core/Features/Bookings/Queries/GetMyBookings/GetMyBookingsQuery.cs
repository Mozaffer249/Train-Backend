using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Core.Features.Bookings.Queries.GetMyBookings
{
    public class GetMyBookingsQuery : IRequest<Response<List<BookingDto>>>
    {
    }
}
