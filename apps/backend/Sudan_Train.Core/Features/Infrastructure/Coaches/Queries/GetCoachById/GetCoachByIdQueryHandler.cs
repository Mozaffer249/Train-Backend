using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Queries.GetCoachById
{
    public class GetCoachByIdQueryHandler : ResponseHandler, IRequestHandler<GetCoachByIdQuery, Response<CoachDto>>
    {
        private readonly ITrainService _trainService;

        public GetCoachByIdQueryHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<CoachDto>> Handle(GetCoachByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _trainService.GetCoachByIdAsync(request.Id);
            if (dto == null)
                return NotFound<CoachDto>($"Coach with ID {request.Id} not found");
            return Success(null, dto);
        }
    }
}
