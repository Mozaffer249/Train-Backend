using FluentValidation;

namespace Sudan_Train.Core.Features.Authentication.Commands.TerminateSession
{
    public class TerminateSessionCommandValidator : AbstractValidator<TerminateSessionCommand>
    {
        public TerminateSessionCommandValidator()
        {
            RuleFor(x => x.SessionId)
                .GreaterThan(0)
                .WithMessage("Session ID is required");
        }
    }
}

