using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.DeleteStation
{
    public class DeleteStationCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}

