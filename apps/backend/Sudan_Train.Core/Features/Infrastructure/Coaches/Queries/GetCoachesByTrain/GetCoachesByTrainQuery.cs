using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Queries.GetCoachesByTrain
{
    public class GetCoachesByTrainQuery : IRequest<Response<List<CoachDto>>>
    {
        public int TrainId { get; set; }
    }
}

