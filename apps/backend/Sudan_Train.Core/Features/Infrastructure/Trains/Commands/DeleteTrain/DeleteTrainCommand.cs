using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Commands.DeleteTrain
{
    public class DeleteTrainCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}

