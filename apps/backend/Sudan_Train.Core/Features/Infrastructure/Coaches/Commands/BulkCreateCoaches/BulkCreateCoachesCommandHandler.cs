using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.BulkCreateCoaches
{
    public class BulkCreateCoachesCommandHandler : ResponseHandler, IRequestHandler<BulkCreateCoachesCommand, Response<List<CoachDto>>>
    {
        private readonly ITrainService _trainService;

        public BulkCreateCoachesCommandHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<List<CoachDto>>> Handle(BulkCreateCoachesCommand request, CancellationToken cancellationToken)
        {
            var coaches = await _trainService.BulkCreateCoachesAsync(
                request.TrainId,
                request.NumberOfCoaches,
                request.Class,
                request.CapacityPerCoach,
                request.AutoGenerateSeats);
            return Success($"{coaches.Count} coaches created successfully", coaches);
        }
    }
}

