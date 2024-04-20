namespace Smart.Ddd.Domain.Repositories;

public class PagedRequestBase
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

public class PagedRequest : PagedRequestBase
{
    public Dictionary<string, bool>? SortFields { get; set; }

    public PagedRequest() { }

    public PagedRequest(int pageNumber, int pageSize, Dictionary<string, bool>? sortFields = null)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortFields = sortFields;
    }

    public PagedRequest(int pageNumber, int pageSize, string sortField, bool desc = false)
        : this(pageNumber, pageSize, new Dictionary<string, bool> { { sortField, desc } }) { }
}
