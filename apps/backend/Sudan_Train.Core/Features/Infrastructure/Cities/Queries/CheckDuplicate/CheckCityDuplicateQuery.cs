using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Queries.CheckDuplicate
{
    public class CheckCityDuplicateQuery : IRequest<Response<bool>>
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public int? ExcludeId { get; set; }
    }
}
