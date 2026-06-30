using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : ResponseHandler, IRequestHandler<CreateBookingCommand, Response<BookingDto>>
    {
        private readonly IBookingService _bookingService;
        private readonly IHttpContextAccessor _http;

        public CreateBookingCommandHandler(
            IBookingService bookingService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _bookingService = bookingService;
            _http = http;
        }

        public async Task<Response<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);

            var input = new CreateBookingInput
            {
                UserId = userId > 0 ? userId : null,
                HoldingUserId = userId > 0 ? userId : null,
                TripId = request.TripId,
                BoardingStationId = request.BoardingStationId,
                AlightingStationId = request.AlightingStationId,
                PaymentMethod = request.PaymentMethod,
                CardLast4 = request.CardLast4,
                Passengers = request.Passengers.Select(ps => new PassengerSeatInput
                {
                    SeatId = ps.SeatId,
                    CoachClass = ps.CoachClass,
                    Passenger = new PassengerInput
                    {
                        FullNameEn = ps.Passenger.FullNameEn,
                        FullNameAr = ps.Passenger.FullNameAr,
                        IdNumber = ps.Passenger.IdNumber,
                        Phone = ps.Passenger.Phone,
                        Email = ps.Passenger.Email,
                        Gender = ps.Passenger.Gender,
                        Nationality = ps.Passenger.Nationality,
                        BirthDate = ps.Passenger.BirthDate,
                    },
                }).ToList(),
            };

            var result = await _bookingService.CreateBookingAsync(input);

            if (result.Conflict)
                return UnprocessableEntity<BookingDto>(result.Error ?? "Seat is no longer available.");
            if (result.NotFound)
                return NotFound<BookingDto>(result.Error ?? "Resource not found.");
            if (result.Invalid)
                return BadRequest<BookingDto>(result.Error ?? "Invalid booking request.");

            return Created("Booking created", result.Booking!);
        }
    }
}
