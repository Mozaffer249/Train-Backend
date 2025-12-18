using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Seats.Queries.GetSeatsByCoach
{
    public class GetSeatsByCoachQueryHandler : ResponseHandler, IRequestHandler<GetSeatsByCoachQuery, Response<List<SeatDto>>>
    {
        private readonly ITrainService _trainService;

        public GetSeatsByCoachQueryHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<List<SeatDto>>> Handle(GetSeatsByCoachQuery request, CancellationToken cancellationToken)
        {
            var seats = await _trainService.GetSeatsByCoachAsync(request.CoachId);
            return Success(null, seats);
        }
    }
}

