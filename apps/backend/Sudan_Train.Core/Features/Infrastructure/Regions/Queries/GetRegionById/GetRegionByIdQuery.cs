using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Queries.GetRegionById
{
    public class GetRegionByIdQuery : IRequest<Response<RegionDto>>
    {
        public int Id { get; set; }
    }
}

