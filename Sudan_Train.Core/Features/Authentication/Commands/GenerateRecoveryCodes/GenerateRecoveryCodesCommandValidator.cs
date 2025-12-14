using FluentValidation;

namespace Sudan_Train.Core.Features.Authentication.Commands.GenerateRecoveryCodes
{
    public class GenerateRecoveryCodesCommandValidator : AbstractValidator<GenerateRecoveryCodesCommand>
    {
        public GenerateRecoveryCodesCommandValidator()
        {
            // No validation rules needed - command is empty and uses authenticated user
        }
    }
}
