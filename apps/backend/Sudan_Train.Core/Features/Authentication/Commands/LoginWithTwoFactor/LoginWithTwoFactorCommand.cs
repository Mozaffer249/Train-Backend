using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.Results;

namespace Sudan_Train.Core.Features.Authentication.Commands.LoginWithTwoFactor
{
    public class LoginWithTwoFactorCommand : IRequest<Response<JwtAuthResult>>
    {
        public string UserName { get; set; } = default!;
        public string Code { get; set; } = default!;
        public bool UseRecoveryCode { get; set; } = false;
    }
}
