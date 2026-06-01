using FluentValidation;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Commands.UpdateFare
{
    public class UpdateFareCommandValidator : AbstractValidator<UpdateFareCommand>
    {
        public UpdateFareCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.BasePrice)
                .GreaterThan(0).When(x => x.BasePrice.HasValue)
                .WithMessage("Base price must be greater than 0");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).When(x => x.DiscountPercent.HasValue)
                .WithMessage("Discount percent must be between 0 and 100");

            RuleFor(x => x)
                .Must(x => !(x.EffectiveFrom.HasValue && x.EffectiveTo.HasValue) || x.EffectiveTo > x.EffectiveFrom)
                .WithMessage("EffectiveTo must be after EffectiveFrom");
        }
    }
}
