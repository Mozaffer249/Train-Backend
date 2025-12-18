using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.UpdateState
{
    public class UpdateStateCommandHandler : ResponseHandler, IRequestHandler<UpdateStateCommand, Response<StateDto>>
    {
        private readonly IGeographyService _geographyService;

        public UpdateStateCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<StateDto>> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var stateDto = await _geographyService.UpdateStateAsync(
                    request.Id,
                    request.NameEn,
                    request.NameAr,
                    request.RegionId);

                return Success("State updated successfully", stateDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<StateDto>("State not found");
            }
        }
    }
}

