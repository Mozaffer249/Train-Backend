using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.CreateCity
{
    public class CreateCityCommandHandler : ResponseHandler, IRequestHandler<CreateCityCommand, Response<CityDto>>
    {
        private readonly IGeographyService _geographyService;

        public CreateCityCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var cityDto = await _geographyService.CreateCityAsync(
                request.NameEn,
                request.NameAr,
                request.Latitude,
                request.Longitude,
                request.GooglePlaceId,
                request.FormattedAddress);

            return Success("City created successfully", cityDto);
        }
    }
}

