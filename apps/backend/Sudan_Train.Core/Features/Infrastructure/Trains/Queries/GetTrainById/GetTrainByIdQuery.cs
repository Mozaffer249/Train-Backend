using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetTrainById
{
    public class GetTrainByIdQuery : IRequest<Response<TrainDto>>
    {
        public int Id { get; set; }
    }
}

