using FluentValidation;

namespace Sudan_Train.Core.Features.Bookings.Commands.CreateCounterBooking
{
    public class CreateCounterBookingCommandValidator : AbstractValidator<CreateCounterBookingCommand>
    {
        public CreateCounterBookingCommandValidator()
        {
            RuleFor(x => x.TripId).GreaterThan(0);
            RuleFor(x => x.BoardingStationId).GreaterThan(0);
            RuleFor(x => x.AlightingStationId).GreaterThan(0)
                .NotEqual(x => x.BoardingStationId)
                .WithMessage("Boarding and alighting stations must differ.");

            RuleFor(x => x.Passengers)
                .NotEmpty().WithMessage("At least one passenger is required.");

            RuleForEach(x => x.Passengers).ChildRules(p =>
            {
                p.RuleFor(ps => ps.SeatId).GreaterThan(0);
                p.RuleFor(ps => ps.Passenger).NotNull();
                p.RuleFor(ps => ps.Passenger.FullNameEn)
                    .NotEmpty().WithMessage("Passenger full name (English) is required.")
                    .MaximumLength(200);
                p.RuleFor(ps => ps.Passenger.IdNumber)
                    .NotEmpty().WithMessage("Passenger ID/passport number is required.")
                    .MaximumLength(50);
                p.RuleFor(ps => ps.Passenger.FullNameAr).MaximumLength(200);
                p.RuleFor(ps => ps.Passenger.Phone).MaximumLength(50);
                p.RuleFor(ps => ps.Passenger.Email).EmailAddress()
                    .When(ps => !string.IsNullOrWhiteSpace(ps.Passenger.Email));
            });
        }
    }
}
