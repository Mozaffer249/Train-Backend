namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class StateDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int RegionId { get; set; }
        public string RegionName { get; set; } = default!;
        public int CitiesCount { get; set; }
    }
}

