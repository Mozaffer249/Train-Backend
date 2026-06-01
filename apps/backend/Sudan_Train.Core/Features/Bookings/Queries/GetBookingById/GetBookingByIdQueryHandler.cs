using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Queries.GetBookingById
{
    public class GetBookingByIdQueryHandler : ResponseHandler, IRequestHandler<GetBookingByIdQuery, Response<BookingDto>>
    {
        private readonly IBookingService _bookingService;
        private readonly IHttpContextAccessor _http;

        public GetBookingByIdQueryHandler(
            IBookingService bookingService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _bookingService = bookingService;
            _http = http;
        }

        public async Task<Response<BookingDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var user = _http.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);

            var isAdmin = user?.IsInRole(Roles.SuperAdmin) == true
                       || user?.IsInRole(Roles.Admin) == true
                       || user?.IsInRole(Roles.Staff) == true;

            var dto = await _bookingService.GetByIdAsync(request.Id, userId > 0 ? userId : null, isAdmin);
            if (dto == null)
                return NotFound<BookingDto>("Booking not found.");

            return Success(null, dto);
        }
    }
}
