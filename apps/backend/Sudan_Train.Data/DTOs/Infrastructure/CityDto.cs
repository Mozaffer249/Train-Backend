namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class CityDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int StateId { get; set; }
        public string StateName { get; set; } = default!;
        public string RegionName { get; set; } = default!;
        public int StationsCount { get; set; }
    }
}

