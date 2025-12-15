using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.ChangeEmail
{
    public class ChangeEmailCommand : IRequest<Response<string>>
    {
        public string NewEmail { get; set; } = default!;
        public string CurrentPassword { get; set; } = default!;
    }
}

