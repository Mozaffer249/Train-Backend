using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.TerminateAllSessions
{
    public class TerminateAllSessionsCommand : IRequest<Response<string>>
    {
        public bool ExceptCurrent { get; set; } = true;
    }
}

