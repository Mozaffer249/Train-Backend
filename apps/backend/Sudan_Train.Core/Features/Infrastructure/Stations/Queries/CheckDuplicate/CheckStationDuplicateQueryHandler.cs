using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.CheckDuplicate
{
    public class CheckStationDuplicateQueryHandler : ResponseHandler,
        IRequestHandler<CheckStationDuplicateQuery, Response<bool>>
    {
        private readonly IStationService _stationService;

        public CheckStationDuplicateQueryHandler(
            IStationService stationService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _stationService = stationService;
        }

        public async Task<Response<bool>> Handle(
            CheckStationDuplicateQuery request,
            CancellationToken cancellationToken)
        {
            var isUnique = await _stationService.IsStationNameUniqueInCityAsync(
                request.NameEn,
                request.NameAr,
                request.CityId,
                request.ExcludeId);

            // Return true if duplicate exists (NOT unique)
            return Success<bool>(null, !isUnique);
        }
    }
}
