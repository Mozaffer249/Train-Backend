using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.UpdateCoach
{
    public class UpdateCoachCommandHandler : ResponseHandler, IRequestHandler<UpdateCoachCommand, Response<CoachDto>>
    {
        private readonly ITrainService _trainService;

        public UpdateCoachCommandHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<CoachDto>> Handle(UpdateCoachCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = await _trainService.UpdateCoachAsync(
                    request.Id,
                    request.CoachNumber,
                    request.Class,
                    request.Sequence);

                if (dto == null)
                    return NotFound<CoachDto>($"Coach with ID {request.Id} not found");

                return Success("Coach updated successfully", dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest<CoachDto>(ex.Message);
            }
        }
    }
}
