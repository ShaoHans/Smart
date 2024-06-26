﻿using System.Data.Common;

namespace Smart.Ddd.Domain.Uow;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IServiceProvider ServiceProvider { get; }

    DbTransaction Transaction { get; }

    /// <summary>
    /// Whether the transaction has been opened
    /// </summary>
    bool TransactionHasBegun { get; }

    /// <summary>
    /// Whether to use transaction, when the value is null, the provider decides whether to use transaction according to the scenario
    /// </summary>
    bool? UseTransaction { get; set; }

    /// <summary>
    /// Disable transaction rollback after failure
    /// </summary>
    bool DisableRollbackOnFailure { get; set; }

    EntityState EntityState { get; set; }

    CommitState CommitState { get; set; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
