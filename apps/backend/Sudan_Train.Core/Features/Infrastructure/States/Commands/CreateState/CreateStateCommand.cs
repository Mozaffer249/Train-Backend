using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.CreateState
{
    public class CreateStateCommand : IRequest<Response<StateDto>>
    {
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int RegionId { get; set; }
    }
}

