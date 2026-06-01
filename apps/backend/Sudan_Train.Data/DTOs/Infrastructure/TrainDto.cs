namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class TrainDto
    {
        public int Id { get; set; }
        public string TrainNumber { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public int CoachesCount { get; set; }
        public int TotalCapacity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CoachDto
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public string CoachNumber { get; set; } = default!;
        public string Class { get; set; } = default!;
        public int Capacity { get; set; }
        public int Sequence { get; set; }
        public int SeatsCount { get; set; }
    }

    public class SeatDto
    {
        public int Id { get; set; }
        public int CoachId { get; set; }
        public string SeatNumber { get; set; } = default!;
        public bool IsWindow { get; set; }
        public bool IsAccessible { get; set; }
    }
}

