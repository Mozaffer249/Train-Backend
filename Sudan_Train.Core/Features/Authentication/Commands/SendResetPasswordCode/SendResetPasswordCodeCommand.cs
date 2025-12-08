using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.SendResetPasswordCode
{
    public class SendResetPasswordCodeCommand : IRequest<Response<string>>
    {
        public string Email { get; set; } = default!;
    }
}
