using System.Linq.Expressions;
using Smart.Ddd.Domain.Entities;

namespace Smart.Ddd.Domain.Repositories;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    #region Add

    ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );

    #endregion

    #region Update

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );

    #endregion

    #region Remove

    Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task RemoveRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );

    Task RemoveAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<int> BulkDeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    #endregion

    #region Find

    Task<TEntity?> FindAsync(
        IEnumerable<KeyValuePair<string, object>> keyValues,
        CancellationToken cancellationToken = default
    );

    Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    #endregion

    #region Get

    Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetListAsync(
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> GetPagedListAsync(
        int skip,
        int take,
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> GetPagedListAsync(
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skip,
        int take,
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default
    );

    Task<PagedResponse<TEntity>> GetPagedListAsync(
        PagedRequest options,
        CancellationToken cancellationToken = default
    );

    Task<PagedResponse<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        PagedRequest options,
        CancellationToken cancellationToken = default
    );

    Task<List<TResult>> GetListAsync<TResult>(string sql, params object[] parameters);

    #endregion
}

public interface IRepository<TEntity, TKey> : IRepository<TEntity>
    where TEntity : class, IEntity<TKey>
    where TKey : IComparable
{
    #region Find

    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    #endregion

    #region Remove

    Task RemoveAsync(TKey id, CancellationToken cancellationToken = default);

    Task RemoveRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    #endregion
}
