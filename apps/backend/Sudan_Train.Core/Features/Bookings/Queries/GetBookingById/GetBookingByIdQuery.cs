using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Core.Features.Bookings.Queries.GetBookingById
{
    public class GetBookingByIdQuery : IRequest<Response<BookingDto>>
    {
        public int Id { get; set; }
    }
}
