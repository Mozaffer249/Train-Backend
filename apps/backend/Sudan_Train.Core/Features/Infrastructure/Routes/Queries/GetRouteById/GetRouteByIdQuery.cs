using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetRouteById
{
    public class GetRouteByIdQuery : IRequest<Response<RouteDto>>
    {
        public int Id { get; set; }
    }
}

