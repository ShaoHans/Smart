using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Smart.Data;

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

        services.AddUowAndRepository<TDbContext>();

        return services;
    }
}
