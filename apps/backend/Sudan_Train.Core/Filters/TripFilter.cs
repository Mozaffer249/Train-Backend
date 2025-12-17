namespace Sudan_Train.Core.Filters
{
    public class TripFilter : PaginatedListFilter
    {
        public int? RouteId { get; set; }
        public int? TrainId { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string? Status { get; set; }
    }
}

