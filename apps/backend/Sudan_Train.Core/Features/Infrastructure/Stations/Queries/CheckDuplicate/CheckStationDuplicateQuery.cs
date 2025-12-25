using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.CheckDuplicate
{
    public class CheckStationDuplicateQuery : IRequest<Response<bool>>
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public int CityId { get; set; }
        public int? ExcludeId { get; set; }
    }
}
