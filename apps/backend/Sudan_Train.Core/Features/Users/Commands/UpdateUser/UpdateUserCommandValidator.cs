using FluentValidation;

namespace Sudan_Train.Core.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.FirstName).MaximumLength(100).When(x => x.FirstName != null);
            RuleFor(x => x.LastName).MaximumLength(100).When(x => x.LastName != null);
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}
