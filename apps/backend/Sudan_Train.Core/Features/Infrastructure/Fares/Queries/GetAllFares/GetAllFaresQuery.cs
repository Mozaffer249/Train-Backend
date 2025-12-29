using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Queries.GetAllFares
{
    public class GetAllFaresQuery : IRequest<Response<List<FareDto>>>
    {
        public int? RouteId { get; set; }
        public CoachClass? CoachClass { get; set; }
    }
}
