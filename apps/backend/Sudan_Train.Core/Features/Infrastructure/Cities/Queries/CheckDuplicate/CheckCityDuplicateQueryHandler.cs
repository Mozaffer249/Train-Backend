using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Queries.CheckDuplicate
{
    public class CheckCityDuplicateQueryHandler : ResponseHandler,
        IRequestHandler<CheckCityDuplicateQuery, Response<bool>>
    {
        private readonly IGeographyService _geographyService;

        public CheckCityDuplicateQueryHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<bool>> Handle(
            CheckCityDuplicateQuery request,
            CancellationToken cancellationToken)
        {
            var isUnique = await _geographyService.IsCityNameUniqueAsync(
                request.NameEn,
                request.NameAr,
                request.ExcludeId);

            // Return true if duplicate exists (NOT unique)
            return Success<bool>(null, !isUnique);
        }
    }
}
