using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail;
using Sudan_Train.Core.Resources.Shared;

namespace Trains.Core.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID is required.");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("OTP code is required.")
                .Length(4)
                .WithMessage("OTP code must be exactly 4 digits.")
                .Matches(@"^\d{4}$")
                .WithMessage("OTP code must contain only numbers.");
        }
    }
}
