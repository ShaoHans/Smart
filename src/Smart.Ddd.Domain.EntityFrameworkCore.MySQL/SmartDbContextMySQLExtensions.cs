using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Smart.Ddd.Domain.EntityFrameworkCore.Uow;
using Smart.Ddd.Domain.Uow;

namespace Smart.Ddd.Domain.EntityFrameworkCore.MySQL;

public static class SmartDbContextMySQLExtensions
{
    public static IServiceCollection AddDbContext<TDbContext>(
        this IServiceCollection services,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null
    )
        where TDbContext : DbContext
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
        return services;
    }
}
