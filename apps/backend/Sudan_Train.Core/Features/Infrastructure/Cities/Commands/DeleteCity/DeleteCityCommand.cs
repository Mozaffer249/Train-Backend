using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.DeleteCity
{
    public class DeleteCityCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}

