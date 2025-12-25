namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class AreaDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public string Code { get; set; } = default!;
        public int GovernoratesCount { get; set; }
    }
}
