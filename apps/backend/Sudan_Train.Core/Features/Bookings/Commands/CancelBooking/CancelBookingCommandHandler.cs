using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : ResponseHandler, IRequestHandler<CancelBookingCommand, Response<string>>
    {
        private readonly IBookingService _bookingService;
        private readonly IHttpContextAccessor _http;

        public CancelBookingCommandHandler(
            IBookingService bookingService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _bookingService = bookingService;
            _http = http;
        }

        public async Task<Response<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var user = _http.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);

            var isAdmin = user?.IsInRole(Roles.SuperAdmin) == true
                       || user?.IsInRole(Roles.Admin) == true
                       || user?.IsInRole(Roles.Staff) == true;

            var ok = await _bookingService.CancelBookingAsync(
                request.BookingId,
                userId > 0 ? userId : null,
                isAdmin,
                request.Reason);

            if (!ok)
                return BadRequest<string>("Booking cannot be cancelled (not found, not yours, or already finalised).");

            return Success<string>("Booking cancelled.");
        }
    }
}
