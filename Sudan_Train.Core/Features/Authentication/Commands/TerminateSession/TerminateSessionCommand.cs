using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.TerminateSession
{
    public class TerminateSessionCommand : IRequest<Response<string>>
    {
        public long SessionId { get; set; }
    }
}

