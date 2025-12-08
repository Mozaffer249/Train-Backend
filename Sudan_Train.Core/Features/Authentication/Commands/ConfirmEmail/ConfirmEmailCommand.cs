using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommand : IRequest<Response<string>>
    {
        public int UserId { get; set; }
        public string Code { get; set; } = default!;
    }
}
