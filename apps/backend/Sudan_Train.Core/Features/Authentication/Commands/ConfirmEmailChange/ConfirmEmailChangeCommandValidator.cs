using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources.Authentication;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmailChange
{
    public class ConfirmEmailChangeCommandValidator : AbstractValidator<ConfirmEmailChangeCommand>
    {
        private readonly IStringLocalizer<AuthenticationResources> _localizer;

        public ConfirmEmailChangeCommandValidator(IStringLocalizer<AuthenticationResources> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID is required");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required");

            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage(_localizer[AuthenticationResourcesKeys.EmailIsRequired])
                .EmailAddress()
                .WithMessage(_localizer[AuthenticationResourcesKeys.EmailInvalidFormat]);
        }
    }
}

