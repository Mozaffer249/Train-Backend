using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmailChange
{
    public class ConfirmEmailChangeCommand : IRequest<Response<string>>
    {
        public int UserId { get; set; }
        public string Token { get; set; } = default!;
        public string NewEmail { get; set; } = default!;
    }
}

