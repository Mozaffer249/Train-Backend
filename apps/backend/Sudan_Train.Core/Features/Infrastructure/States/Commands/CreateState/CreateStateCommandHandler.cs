using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.CreateState
{
    public class CreateStateCommandHandler : ResponseHandler, IRequestHandler<CreateStateCommand, Response<StateDto>>
    {
        private readonly IGeographyService _geographyService;

        public CreateStateCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<StateDto>> Handle(CreateStateCommand request, CancellationToken cancellationToken)
        {
            var stateDto = await _geographyService.CreateStateAsync(
                request.NameEn,
                request.NameAr,
                request.RegionId);

            return Success("State created successfully", stateDto);
        }
    }
}

