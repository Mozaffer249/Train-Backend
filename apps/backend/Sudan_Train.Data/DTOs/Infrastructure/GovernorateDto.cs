namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class GovernorateDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int AreaId { get; set; }
        public string AreaName { get; set; } = default!;
        public int CitiesCount { get; set; }
    }
}

