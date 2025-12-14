using FluentValidation;

namespace Sudan_Train.Core.Features.Authentication.Commands.VerifyTwoFactor
{
    public class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
    {
        public VerifyTwoFactorCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Verification code is required")
                .Length(6).WithMessage("Verification code must be 6 digits")
                .Matches("^[0-9]{6}$").WithMessage("Verification code must contain only digits");
        }
    }
}
