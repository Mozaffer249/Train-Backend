using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources.Authentication;

namespace Sudan_Train.Core.Features.Authentication.Commands.LoginWithTwoFactor
{
    public class LoginWithTwoFactorCommandValidator : AbstractValidator<LoginWithTwoFactorCommand>
    {
        public LoginWithTwoFactorCommandValidator(IStringLocalizer<AuthenticationResources> localizer)
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(localizer[AuthenticationResourcesKeys.UserNameIsRequired]);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Two-factor code is required");
        }
    }
}
