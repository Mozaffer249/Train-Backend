using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.VerifyTwoFactor
{
    public class VerifyTwoFactorCommand : IRequest<Response<string>>
    {
        public string Code { get; set; } = default!;
    }
}
