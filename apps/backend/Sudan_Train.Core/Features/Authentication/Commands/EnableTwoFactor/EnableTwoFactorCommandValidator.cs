using FluentValidation;

namespace Sudan_Train.Core.Features.Authentication.Commands.EnableTwoFactor
{
    public class EnableTwoFactorCommandValidator : AbstractValidator<EnableTwoFactorCommand>
    {
        public EnableTwoFactorCommandValidator()
        {
            // No validation rules needed - command is empty and uses authenticated user
        }
    }
}
