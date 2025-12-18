using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.DeleteState
{
    public class DeleteStateCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}

