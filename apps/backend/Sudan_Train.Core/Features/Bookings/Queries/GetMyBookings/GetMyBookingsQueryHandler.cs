using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Queries.GetMyBookings
{
    public class GetMyBookingsQueryHandler : ResponseHandler, IRequestHandler<GetMyBookingsQuery, Response<List<BookingDto>>>
    {
        private readonly IBookingService _bookingService;
        private readonly IHttpContextAccessor _http;

        public GetMyBookingsQueryHandler(
            IBookingService bookingService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _bookingService = bookingService;
            _http = http;
        }

        public async Task<Response<List<BookingDto>>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized<List<BookingDto>>("User not authenticated.");

            var bookings = await _bookingService.GetMineAsync(userId);
            return Success(null, bookings);
        }
    }
}
