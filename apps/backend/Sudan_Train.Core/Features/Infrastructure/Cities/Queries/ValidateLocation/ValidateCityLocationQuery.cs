using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Queries.ValidateLocation
{
    public class ValidateCityLocationQuery : IRequest<Response<CityValidationDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
