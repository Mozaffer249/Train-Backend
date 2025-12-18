using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetAllTrains
{
    public class GetAllTrainsQueryHandler : ResponseHandler, IRequestHandler<GetAllTrainsQuery, Response<List<TrainDto>>>
    {
        private readonly ITrainService _trainService;

        public GetAllTrainsQueryHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<List<TrainDto>>> Handle(GetAllTrainsQuery request, CancellationToken cancellationToken)
        {
            var trains = await _trainService.GetAllTrainsAsync(request.SearchTerm);
            return Success(null, trains);
        }
    }
}

