using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetAllTrains
{
    public class GetAllTrainsQuery : IRequest<Response<List<TrainDto>>>
    {
        public string? SearchTerm { get; set; }
    }
}

