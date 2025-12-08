using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources;

namespace Sudan_Train.Core.Features.Authentication.Commands.SendResetPasswordCode
{
    public class SendResetPasswordCodeCommandValidator : AbstractValidator<SendResetPasswordCodeCommand>
    {
        public SendResetPasswordCodeCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .EmailAddress().WithMessage(stringLocalizer[SharedResourcesKeys.InvalidFormat]);
        }
    }
}
