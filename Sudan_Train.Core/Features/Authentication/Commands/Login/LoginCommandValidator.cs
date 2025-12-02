using FluentValidation;

namespace Sudan_Train.Core.Features.Authentication.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .NotNull().WithMessage("Username cannot be null");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .NotNull().WithMessage("Password cannot be null");
        }
    }
}

