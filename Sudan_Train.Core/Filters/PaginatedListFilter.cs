namespace Sudan_Train.Core.Filters
{
    public class PaginatedListFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; } = default!;
        public string Search { get; set; } = default!;

        public PaginatedListFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public PaginatedListFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 100 ? 100 : pageSize;
        }
    }
}

