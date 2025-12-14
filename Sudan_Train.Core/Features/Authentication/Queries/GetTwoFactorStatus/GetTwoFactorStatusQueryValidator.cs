using FluentValidation;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetTwoFactorStatus
{
    public class GetTwoFactorStatusQueryValidator : AbstractValidator<GetTwoFactorStatusQuery>
    {
        public GetTwoFactorStatusQueryValidator()
        {
            // No validation rules needed - query is empty and uses authenticated user
        }
    }
}
