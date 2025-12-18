using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Commands.CreateTrain
{
    public class CreateTrainCommandHandler : ResponseHandler, IRequestHandler<CreateTrainCommand, Response<TrainDto>>
    {
        private readonly ITrainService _trainService;

        public CreateTrainCommandHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<TrainDto>> Handle(CreateTrainCommand request, CancellationToken cancellationToken)
        {
            var trainDto = await _trainService.CreateTrainAsync(
                request.TrainNumber,
                request.NameEn,
                request.NameAr,
                request.Type);
            return Success("Train created successfully", trainDto);
        }
    }
}

