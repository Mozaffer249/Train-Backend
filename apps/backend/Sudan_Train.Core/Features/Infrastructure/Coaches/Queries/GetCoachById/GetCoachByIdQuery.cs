using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Queries.GetCoachById
{
    public class GetCoachByIdQuery : IRequest<Response<CoachDto>>
    {
        public int Id { get; set; }
    }
}
