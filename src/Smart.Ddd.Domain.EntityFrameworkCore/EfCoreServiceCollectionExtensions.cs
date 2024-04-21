using Microsoft.EntityFrameworkCore;
using Smart.Data;
using Smart.Ddd.Domain.EntityFrameworkCore.Uow;
using Smart.Ddd.Domain.Uow;

namespace Microsoft.Extensions.DependencyInjection;

public static class EfCoreServiceCollectionExtensions
{
    public static IServiceCollection AddUowAndRepository<TDbContext>(
        this IServiceCollection services
    )
        where TDbContext : DbContext, ISmartDbContext
    {
        services.AddScoped<IUnitOfWork, EfCoreUnitOfWork<TDbContext>>();

        var entityTypeAssemblies = typeof(TDbContext)
            .GetProperties()
            .Where(p =>
                p.PropertyType.IsGenericType
                && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
            )
            .Select(p => p.PropertyType.GenericTypeArguments[0].Assembly)
            .Distinct();

        services.TryAddRepository<TDbContext>(entityTypeAssemblies);
        return services;
    }
}
