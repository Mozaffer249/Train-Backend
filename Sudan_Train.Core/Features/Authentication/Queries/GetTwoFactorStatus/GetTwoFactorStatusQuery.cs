using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetTwoFactorStatus
{
    public class GetTwoFactorStatusQuery : IRequest<Response<TwoFactorStatusResponse>>
    {
    }

    public class TwoFactorStatusResponse
    {
        public bool IsEnabled { get; set; }
        public bool HasAuthenticatorKey { get; set; }
        public int RecoveryCodesLeft { get; set; }
    }
}
