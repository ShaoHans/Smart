using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Smart.Data;
using Smart.Ddd.Domain.EntityFrameworkCore.Uow;
using Smart.Ddd.Domain.Uow;

namespace Microsoft.Extensions.DependencyInjection;

public static class SmartDbContextMySQLExtensions
{
    public static IServiceCollection AddSmartDbContext<TDbContext>(
        this IServiceCollection services,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null
    )
        where TDbContext : DbContext, ISmartDbContext
    {
        services.AddDbContext<TDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
#if DEBUG
            // The following three options help with debugging, but should
            // be changed or removed for production.
            dbContextOptions.EnableSensitiveDataLogging().EnableDetailedErrors();
#endif

            mySqlOptionsAction?.Invoke(new MySqlDbContextOptionsBuilder(dbContextOptions));
        });

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
