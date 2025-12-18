using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.DeleteCity
{
    public class DeleteCityCommandHandler : ResponseHandler, IRequestHandler<DeleteCityCommand, Response<string>>
    {
        private readonly IGeographyService _geographyService;

        public DeleteCityCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<string>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            var hasStations = await _geographyService.CityHasStationsAsync(request.Id);
            if (hasStations)
                return BadRequest<string>("Cannot delete city because it has stations");

            var deleted = await _geographyService.DeleteCityAsync(request.Id);
            if (!deleted)
                return NotFound<string>("City not found");

            return Success<string>("City deleted successfully");
        }
    }
}

