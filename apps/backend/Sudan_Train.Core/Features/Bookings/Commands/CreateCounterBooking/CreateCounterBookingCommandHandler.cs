using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Commands.CreateCounterBooking
{
    public class CreateCounterBookingCommandHandler
        : ResponseHandler, IRequestHandler<CreateCounterBookingCommand, Response<BookingDto>>
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _http;
        private readonly IStaffAuthorizationService _staffAuth;

        public CreateCounterBookingCommandHandler(
            IBookingService bookingService,
            UserManager<User> userManager,
            IHttpContextAccessor http,
            IStaffAuthorizationService staffAuth,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _bookingService = bookingService;
            _userManager = userManager;
            _http = http;
            _staffAuth = staffAuth;
        }

        public async Task<Response<BookingDto>> Handle(CreateCounterBookingCommand request, CancellationToken cancellationToken)
        {
            // If a CustomerUserId is supplied it must reference an active,
            // non-staff account. Walk-ins (null) bypass this check.
            if (request.CustomerUserId.HasValue)
            {
                var customer = await _userManager.FindByIdAsync(request.CustomerUserId.Value.ToString());
                if (customer == null)
                    return NotFound<BookingDto>("Selected customer not found.");
                if (!customer.IsActive)
                    return BadRequest<BookingDto>("Selected customer account is disabled.");
            }

            // Station scope for non-admin callers. StaffCounter agents may only
            // sell tickets for trips touching their assigned stations AND only
            // when the booking's BoardingStationId is one of those stations
            // (the customer is physically at the agent's counter).
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);
            var roles = _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? new List<string>();

            if (userId > 0 && !_staffAuth.IsAdmin(roles))
            {
                if (!await _staffAuth.CanOperateTripAsync(userId, roles, request.TripId))
                    return Unauthorized<BookingDto>("Trip does not touch any of your assigned stations.");

                var assignedIds = await _staffAuth.GetAssignedStationIdsAsync(userId);
                if (!assignedIds.Contains(request.BoardingStationId))
                    return BadRequest<BookingDto>("Boarding station must be one of your assigned stations.");
            }

            var input = new CreateBookingInput
            {
                UserId = request.CustomerUserId,
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

            return Created("Counter booking created", result.Booking!);
        }
    }
}
