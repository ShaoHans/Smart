using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Smart.Ddd.Domain.Uow;
using EntityState = Smart.Ddd.Domain.Uow.EntityState;

namespace Smart.Ddd.Domain.EntityFrameworkCore.Uow;

public class EfCoreUnitOfWork<TDbContext>(IServiceProvider serviceProvider) : IUnitOfWork
    where TDbContext : DbContext
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    private DbContext? _context;

    protected DbContext Context => _context ??= ServiceProvider.GetRequiredService<TDbContext>();

    public DbTransaction Transaction
    {
        get
        {
            if (UseTransaction is false)
                throw new NotSupportedException("Doesn't support transaction opening");

            if (TransactionHasBegun)
                return Context.Database.CurrentTransaction!.GetDbTransaction();

            return Context.Database.BeginTransaction().GetDbTransaction();
        }
    }

    public bool TransactionHasBegun => Context.Database.CurrentTransaction != null;

    public bool DisableRollbackOnFailure { get; set; }

    public EntityState EntityState { get; set; } = EntityState.UnChanged;

    public CommitState CommitState { get; set; } = CommitState.Commited;

    public bool? UseTransaction { get; set; } = null;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (EntityState == EntityState.UnChanged)
            return;

        await Context.SaveChangesAsync(cancellationToken);
        EntityState = EntityState.UnChanged;
    }

    public async ValueTask DisposeAsync()
    {
        DisposeAsync(true);
        await (_context?.DisposeAsync() ?? ValueTask.CompletedTask);
        GC.SuppressFinalize(this);
    }

    protected virtual void DisposeAsync(bool disposing) { }

    public void Dispose()
    {
        Dispose(true);
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) { }
}
