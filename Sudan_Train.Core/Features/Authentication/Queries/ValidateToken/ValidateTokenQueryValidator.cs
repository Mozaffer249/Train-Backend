using FluentValidation;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Features.Authentication.Queries.ValidateToken;
using Sudan_Train.Core.Resources.Shared;

namespace Trains.Core.Features.Authentication.Queries.ValidateToken
{
    public class ValidateTokenQueryValidator : AbstractValidator<ValidateTokenQuery>
    {
        public ValidateTokenQueryValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired])
                .OverridePropertyName(string.Empty);
        }
    }
}
