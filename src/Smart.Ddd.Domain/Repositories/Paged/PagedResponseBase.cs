using Smart.Ddd.Domain.Entities;

namespace Smart.Ddd.Domain.Repositories;

public class PagedResponseBase<TEntity>
    where TEntity : class
{
    public long TotalCount { get; set; }

    public int TotalPages { get; set; }

    public List<TEntity> Result { get; set; } = default!;
}

public class PagedResponse<TEntity> : PagedResponseBase<TEntity>
    where TEntity : class, IEntity { }
