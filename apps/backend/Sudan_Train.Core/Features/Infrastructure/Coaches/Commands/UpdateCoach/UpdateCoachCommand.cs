using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.UpdateCoach
{
    // PATCH-style coach update. Capacity is intentionally not editable here.
    public class UpdateCoachCommand : IRequest<Response<CoachDto>>
    {
        public int Id { get; set; }
        public string? CoachNumber { get; set; }
        public CoachClass? Class { get; set; }
        public int? Sequence { get; set; }
    }
}
