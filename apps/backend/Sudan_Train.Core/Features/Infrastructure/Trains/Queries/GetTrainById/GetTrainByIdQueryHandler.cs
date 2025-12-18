using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetTrainById
{
    public class GetTrainByIdQueryHandler : ResponseHandler, IRequestHandler<GetTrainByIdQuery, Response<TrainDto>>
    {
        private readonly ITrainService _trainService;

        public GetTrainByIdQueryHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<TrainDto>> Handle(GetTrainByIdQuery request, CancellationToken cancellationToken)
        {
            var train = await _trainService.GetTrainByIdAsync(request.Id);
            if (train == null)
                return NotFound<TrainDto>("Train not found");

            return Success(null, train);
        }
    }
}

