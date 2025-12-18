using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.States.Queries.GetAllStates
{
    public class GetAllStatesQuery : IRequest<Response<List<StateDto>>>
    {
        public int? RegionId { get; set; }
    }
}

