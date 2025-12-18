using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Commands.DeleteRegion
{
    public class DeleteRegionCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}
