using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Queries.GetCoachesByTrain
{
    public class GetCoachesByTrainQueryHandler : ResponseHandler, IRequestHandler<GetCoachesByTrainQuery, Response<List<CoachDto>>>
    {
        private readonly ITrainService _trainService;

        public GetCoachesByTrainQueryHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<List<CoachDto>>> Handle(GetCoachesByTrainQuery request, CancellationToken cancellationToken)
        {
            var coaches = await _trainService.GetCoachesByTrainAsync(request.TrainId);
            return Success(null, coaches);
        }
    }
}

