namespace Sudan_Train.Core.Filters
{
    public class BookingFilter : PaginatedListFilter
    {
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

