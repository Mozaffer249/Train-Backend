using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetStationById
{
    public class GetStationByIdQuery : IRequest<Response<StationDto>>
    {
        public int Id { get; set; }
    }
}

