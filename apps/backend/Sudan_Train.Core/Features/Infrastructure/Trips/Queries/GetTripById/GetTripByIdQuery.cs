using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetTripById
{
    public class GetTripByIdQuery : IRequest<Response<TripDto>>
    {
        public int Id { get; set; }
    }
}

