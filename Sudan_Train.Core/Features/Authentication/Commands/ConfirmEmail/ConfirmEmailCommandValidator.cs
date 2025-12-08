using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired]);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired]);
        }
    }
}
