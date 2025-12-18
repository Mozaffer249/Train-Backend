using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Queries.GetCityById
{
    public class GetCityByIdQuery : IRequest<Response<CityDto>>
    {
        public int Id { get; set; }
    }
}

