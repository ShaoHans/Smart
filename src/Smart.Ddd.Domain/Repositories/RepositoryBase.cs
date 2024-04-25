using System.Linq.Expressions;
using Smart.Ddd.Domain.Entities;
using Smart.Ddd.Domain.Uow;

namespace Smart.Ddd.Domain.Repositories;

public abstract class RepositoryBase<TEntity>(IServiceProvider serviceProvider)
    : IRepository<TEntity>
    where TEntity : class, IEntity
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public virtual EntityState EntityState
    {
        get => UnitOfWork.EntityState;
        set => UnitOfWork.EntityState = value;
    }

    public virtual CommitState CommitState
    {
        get => UnitOfWork.CommitState;
        protected set => UnitOfWork.CommitState = value;
    }

    public abstract IUnitOfWork UnitOfWork { get; }

    #region IRepository<TEntity>

    public abstract ValueTask<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    );

    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity, cancellationToken);
        }
    }

    public abstract Task<TEntity?> FindAsync(
        IEnumerable<KeyValuePair<string, object>> keyValues,
        CancellationToken cancellationToken = default
    );

    public abstract Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    public abstract Task<TEntity> RemoveAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    );

    public abstract Task RemoveAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    public virtual async Task RemoveRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var entity in entities)
        {
            await RemoveAsync(entity, cancellationToken);
        }
    }

    public abstract Task<int> BulkDeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    public abstract Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    );

    public virtual async Task UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var entity in entities)
        {
            await UpdateAsync(entity, cancellationToken);
        }
    }

    public abstract Task<IEnumerable<TEntity>> GetListAsync(
        CancellationToken cancellationToken = default
    );

    public abstract Task<IEnumerable<TEntity>> GetListAsync(
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    public abstract Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    public abstract Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    public abstract Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    public abstract Task<long> GetCountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    public abstract Task<List<TEntity>> GetPagedListAsync(
        int skip,
        int take,
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    public abstract Task<List<TEntity>> GetPagedListAsync(
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default
    );

    public abstract Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skip,
        int take,
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    public abstract Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default
    );

    public virtual async Task<PagedResponse<TEntity>> GetPagedListAsync(
        PagedRequest options,
        CancellationToken cancellationToken = default
    )
    {
        var result = await GetPagedListAsync(
            (options.PageNumber - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.SortFields,
            cancellationToken
        );

        var total = await GetCountAsync(cancellationToken);

        return new PagedResponse<TEntity>()
        {
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize),
            Result = result,
        };
    }

    public virtual async Task<PagedResponse<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        PagedRequest options,
        CancellationToken cancellationToken = default
    )
    {
        var result = await GetPagedListAsync(
            predicate,
            (options.PageNumber - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.SortFields,
            cancellationToken
        );

        var total = await GetCountAsync(predicate, cancellationToken);

        return new PagedResponse<TEntity>()
        {
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize),
            Result = result
        };
    }

    public abstract Task<List<TResult>> SqlQueryAsync<TResult>(string sql, params object[] parameters);

    #endregion
}
