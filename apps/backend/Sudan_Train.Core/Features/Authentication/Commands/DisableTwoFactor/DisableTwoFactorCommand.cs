using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.DisableTwoFactor
{
    public class DisableTwoFactorCommand : IRequest<Response<string>>
    {
        public string Password { get; set; } = default!;
    }
}
