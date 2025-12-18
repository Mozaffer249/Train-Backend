using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.UpdateState
{
    public class UpdateStateCommand : IRequest<Response<StateDto>>
    {
        public int Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public int? RegionId { get; set; }
    }
}

