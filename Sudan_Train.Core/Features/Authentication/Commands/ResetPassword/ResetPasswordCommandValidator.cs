using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Features.Authentication.Commands.ResetPassword;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Core.Resources.Shared;

namespace Trains.Core.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator(
            IStringLocalizer<AuthenticationResources> authLocalizer,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(authLocalizer[AuthenticationResourcesKeys.EmailIsRequired])
                .NotNull().WithMessage(authLocalizer[AuthenticationResourcesKeys.EmailIsRequired])
                .EmailAddress().WithMessage(authLocalizer[AuthenticationResourcesKeys.EmailInvalidFormat])
                .OverridePropertyName(string.Empty);

            RuleFor(x => x.ResetCode)
                .NotEmpty().WithMessage(sharedLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(sharedLocalizer[SharedResourcesKeys.IsRequired])
                .OverridePropertyName(string.Empty);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(authLocalizer[AuthenticationResourcesKeys.PasswordIsRequired])
                .NotNull().WithMessage(authLocalizer[AuthenticationResourcesKeys.PasswordIsRequired])
                .MinimumLength(6).WithMessage(authLocalizer[AuthenticationResourcesKeys.PasswordMinLength])
                .OverridePropertyName(string.Empty);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(authLocalizer[AuthenticationResourcesKeys.ConfirmPasswordIsRequired])
                .NotNull().WithMessage(authLocalizer[AuthenticationResourcesKeys.ConfirmPasswordIsRequired])
                .Equal(x => x.NewPassword).WithMessage(authLocalizer[AuthenticationResourcesKeys.PasswordsDoNotMatch])
                .OverridePropertyName(string.Empty);
        }
    }
}
