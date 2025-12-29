using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Queries.GetAllFares
{
    public class GetAllFaresQueryHandler : ResponseHandler, IRequestHandler<GetAllFaresQuery, Response<List<FareDto>>>
    {
        private readonly IFareService _fareService;

        public GetAllFaresQueryHandler(
            IFareService fareService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _fareService = fareService;
        }

        public async Task<Response<List<FareDto>>> Handle(GetAllFaresQuery request, CancellationToken cancellationToken)
        {
            var fares = await _fareService.GetAllFaresAsync(request.RouteId, request.CoachClass);
            return Success(null, fares);
        }
    }
}
