using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Queries.GetCityById
{
    public class GetCityByIdQueryHandler : ResponseHandler, IRequestHandler<GetCityByIdQuery, Response<CityDto>>
    {
        private readonly IGeographyService _geographyService;

        public GetCityByIdQueryHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var city = await _geographyService.GetCityByIdAsync(request.Id);
            if (city == null)
                return NotFound<CityDto>("City not found");

            return Success(null, city);
        }
    }
}

