using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Commands.CreateRegion
{
    public class CreateRegionCommand : IRequest<Response<RegionDto>>
    {
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public string Code { get; set; } = default!;
    }
}
