using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Commands.UpdateTrain
{
    public class UpdateTrainCommandHandler : ResponseHandler, IRequestHandler<UpdateTrainCommand, Response<TrainDto>>
    {
        private readonly ITrainService _trainService;

        public UpdateTrainCommandHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<TrainDto>> Handle(UpdateTrainCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var trainDto = await _trainService.UpdateTrainAsync(
                    request.Id,
                    request.TrainNumber,
                    request.NameEn,
                    request.NameAr,
                    request.Type);
                return Success("Train updated successfully", trainDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<TrainDto>("Train not found");
            }
        }
    }
}

