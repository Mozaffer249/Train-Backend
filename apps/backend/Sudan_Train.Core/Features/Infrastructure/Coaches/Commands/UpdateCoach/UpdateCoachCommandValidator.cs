using FluentValidation;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.UpdateCoach
{
    public class UpdateCoachCommandValidator : AbstractValidator<UpdateCoachCommand>
    {
        public UpdateCoachCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.CoachNumber)
                .NotEmpty().MaximumLength(20)
                .When(x => x.CoachNumber != null)
                .WithMessage("Coach number cannot be empty when supplied");

            RuleFor(x => x.Sequence)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Sequence.HasValue)
                .WithMessage("Sequence cannot be negative");
        }
    }
}
