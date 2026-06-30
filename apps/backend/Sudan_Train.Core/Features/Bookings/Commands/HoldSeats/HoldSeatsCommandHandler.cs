using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Commands.HoldSeats
{
    public class HoldSeatsCommandHandler : ResponseHandler, IRequestHandler<HoldSeatsCommand, Response<SeatHoldResultDto>>
    {
        private readonly ISeatHoldService _seatHoldService;
        private readonly IHttpContextAccessor _http;

        public HoldSeatsCommandHandler(
            ISeatHoldService seatHoldService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _seatHoldService = seatHoldService;
            _http = http;
        }

        public async Task<Response<SeatHoldResultDto>> Handle(HoldSeatsCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized<SeatHoldResultDto>("Authentication required.");

            var result = await _seatHoldService.HoldSeatsAsync(
                userId,
                request.TripId,
                request.BoardingStationId,
                request.AlightingStationId,
                request.SeatIds,
                request.HoldGroupId);

            if (result.Conflict)
                return UnprocessableEntity<SeatHoldResultDto>(result.Error ?? "Seat is not available.");
            if (!result.Success)
                return BadRequest<SeatHoldResultDto>(result.Error ?? "Could not hold seats.");

            return Success("Seats held", result.Data!);
        }
    }
}
