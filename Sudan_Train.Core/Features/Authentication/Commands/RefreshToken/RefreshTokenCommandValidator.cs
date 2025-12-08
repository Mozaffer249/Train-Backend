using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Resources;

namespace Sudan_Train.Core.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired]);

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired]);
        }
    }
}
