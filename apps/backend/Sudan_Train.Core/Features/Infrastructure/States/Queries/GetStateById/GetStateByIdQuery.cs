using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.States.Queries.GetStateById
{
    public class GetStateByIdQuery : IRequest<Response<StateDto>>
    {
        public int Id { get; set; }
    }
}

