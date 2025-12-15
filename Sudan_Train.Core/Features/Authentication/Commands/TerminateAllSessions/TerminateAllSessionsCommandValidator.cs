using FluentValidation;

namespace Sudan_Train.Core.Features.Authentication.Commands.TerminateAllSessions
{
    public class TerminateAllSessionsCommandValidator : AbstractValidator<TerminateAllSessionsCommand>
    {
        public TerminateAllSessionsCommandValidator()
        {
            // No specific validation needed, ExceptCurrent has a default value
        }
    }
}

