using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources.Authentication;

namespace Sudan_Train.Core.Features.Authentication.Commands.ChangeEmail
{
    public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
    {
        private readonly IStringLocalizer<AuthenticationResources> _localizer;

        public ChangeEmailCommandValidator(IStringLocalizer<AuthenticationResources> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage(_localizer[AuthenticationResourcesKeys.EmailIsRequired])
                .EmailAddress()
                .WithMessage(_localizer[AuthenticationResourcesKeys.EmailInvalidFormat]);

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(_localizer[AuthenticationResourcesKeys.PasswordIsRequired]);
        }
    }
}

