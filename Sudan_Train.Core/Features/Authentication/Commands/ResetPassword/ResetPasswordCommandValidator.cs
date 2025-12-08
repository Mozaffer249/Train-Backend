using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources;

namespace Sudan_Train.Core.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .EmailAddress().WithMessage(stringLocalizer[SharedResourcesKeys.InvalidFormat]);

            RuleFor(x => x.ResetCode)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired]);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
        }
    }
}
