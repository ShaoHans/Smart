using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Smart.Data;
using Smart.Ddd.Domain.EntityFrameworkCore.Uow;
using Smart.Ddd.Domain.Uow;

using System.Reflection;

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
            dbContextOptions.UseMySql(ServerVersion.AutoDetect(connectionString));
#if DEBUG
            // The following three options help with debugging, but should
            // be changed or removed for production.
            dbContextOptions
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
#endif

            mySqlOptionsAction?.Invoke(new MySqlDbContextOptionsBuilder(dbContextOptions));
        });

        services.AddScoped<IUnitOfWork, EfCoreUnitOfWork<TDbContext>>();

        services.TryAddRepository<TDbContext>([Assembly.GetExecutingAssembly()]);

        return services;
    }
}
