using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Queries.GetAllBookings
{
    public class GetAllBookingsQueryHandler : ResponseHandler, IRequestHandler<GetAllBookingsQuery, Response<List<BookingDto>>>
    {
        private readonly IBookingService _bookingService;

        public GetAllBookingsQueryHandler(
            IBookingService bookingService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _bookingService = bookingService;
        }

        public async Task<Response<List<BookingDto>>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingService.GetAllAsync(new BookingListParams
            {
                Status = request.Status,
                UserId = request.UserId,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            });
            return Success(null, bookings);
        }
    }
}
