using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.BulkCreateCoaches
{
    public class BulkCreateCoachesCommand : IRequest<Response<List<CoachDto>>>
    {
        public int TrainId { get; set; }
        public int NumberOfCoaches { get; set; }
        public CoachClass Class { get; set; }
        public int CapacityPerCoach { get; set; }
        public bool AutoGenerateSeats { get; set; } = true;
    }
}

