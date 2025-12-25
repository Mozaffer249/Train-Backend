using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.UpdateCity
{
    public class UpdateCityCommandHandler : ResponseHandler, IRequestHandler<UpdateCityCommand, Response<CityDto>>
    {
        private readonly IGeographyService _geographyService;

        public UpdateCityCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cityDto = await _geographyService.UpdateCityAsync(
                    request.Id,
                    request.NameEn,
                    request.NameAr,
                    request.Latitude,
                    request.Longitude,
                    request.GooglePlaceId,
                    request.FormattedAddress);

                return Success("City updated successfully", cityDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<CityDto>("City not found");
            }
        }
    }
}

