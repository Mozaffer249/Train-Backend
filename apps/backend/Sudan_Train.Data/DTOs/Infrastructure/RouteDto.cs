namespace Sudan_Train.Data.DTOs.Infrastructure
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public StationDto Origin { get; set; } = default!;
        public StationDto Destination { get; set; } = default!;
        public decimal? DistanceKm { get; set; }
        public List<RouteStationDto> IntermediateStops { get; set; } = new();
        public int TripsCount { get; set; }
    }

    public class RouteStationDto
    {
        public int Id { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; } = default!;
        public int StopOrder { get; set; }
        public TimeSpan? ArrivalOffset { get; set; }
        public TimeSpan? DepartureOffset { get; set; }
    }
}

