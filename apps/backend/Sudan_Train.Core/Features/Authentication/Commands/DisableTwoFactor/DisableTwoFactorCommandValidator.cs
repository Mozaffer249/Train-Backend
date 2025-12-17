using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources.Authentication;

namespace Sudan_Train.Core.Features.Authentication.Commands.DisableTwoFactor
{
    public class DisableTwoFactorCommandValidator : AbstractValidator<DisableTwoFactorCommand>
    {
        public DisableTwoFactorCommandValidator(IStringLocalizer<AuthenticationResources> localizer)
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer[AuthenticationResourcesKeys.PasswordIsRequired]);
        }
    }
}
