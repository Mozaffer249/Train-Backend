using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.DeleteRoute
{
    public class DeleteRouteCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}

