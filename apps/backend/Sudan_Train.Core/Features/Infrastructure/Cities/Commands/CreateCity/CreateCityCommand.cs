using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.CreateCity
{
    public class CreateCityCommand : IRequest<Response<CityDto>>
    {
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int StateId { get; set; }
    }
}

