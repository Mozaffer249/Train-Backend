using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Core.Features.Bookings.Queries.GetAllBookings
{
    public class GetAllBookingsQuery : IRequest<Response<List<BookingDto>>>
    {
        public string? Status { get; set; }
        public int? UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
