using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Queries.GetAllCities
{
    public class GetAllCitiesQueryHandler : ResponseHandler, IRequestHandler<GetAllCitiesQuery, Response<List<CityDto>>>
    {
        private readonly IGeographyService _geographyService;

        public GetAllCitiesQueryHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<List<CityDto>>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
        {
            var cities = await _geographyService.GetAllCitiesAsync();
            return Success(null, cities);
        }
    }
}

