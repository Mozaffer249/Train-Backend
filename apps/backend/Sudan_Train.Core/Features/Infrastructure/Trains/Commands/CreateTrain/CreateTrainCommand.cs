using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Commands.CreateTrain
{
    public class CreateTrainCommand : IRequest<Response<TrainDto>>
    {
        public string TrainNumber { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public CoachClass Type { get; set; }
    }
}

