using FluentValidation;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Commands.CreateFare
{
    public class CreateFareCommandValidator : AbstractValidator<CreateFareCommand>
    {
        public CreateFareCommandValidator()
        {
            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage("Base price must be greater than 0");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).When(x => x.DiscountPercent.HasValue)
                .WithMessage("Discount percent must be between 0 and 100");

            RuleFor(x => x)
                .Must(HaveAtLeastOneScope).WithMessage("Must specify at least one of: RouteId, Segment (Origin + Destination), or TripId");
        }

        private bool HaveAtLeastOneScope(CreateFareCommand command)
        {
            return command.RouteId.HasValue ||
                   (command.OriginStationId.HasValue && command.DestinationStationId.HasValue) ||
                   command.TripId.HasValue;
        }
    }
}
