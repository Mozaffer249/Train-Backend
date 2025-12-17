using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources.Authentication;

namespace Sudan_Train.Core.Features.Authentication.Commands.DeleteAccount
{
    public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
    {
        private readonly IStringLocalizer<AuthenticationResources> _localizer;

        public DeleteAccountCommandValidator(IStringLocalizer<AuthenticationResources> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(_localizer[AuthenticationResourcesKeys.PasswordIsRequired]);

            RuleFor(x => x.ConfirmDeletion)
                .Equal(true)
                .WithMessage("Account deletion must be confirmed");
        }
    }
}

