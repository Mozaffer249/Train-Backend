using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Seats.Queries.GetSeatsByCoach
{
    public class GetSeatsByCoachQuery : IRequest<Response<List<SeatDto>>>
    {
        public int CoachId { get; set; }
    }
}

