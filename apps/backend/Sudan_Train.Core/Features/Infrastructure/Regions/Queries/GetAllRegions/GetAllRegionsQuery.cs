using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Queries.GetAllRegions
{
    public class GetAllRegionsQuery : IRequest<Response<List<RegionDto>>>
    {
    }
}

