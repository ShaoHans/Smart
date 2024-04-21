using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sample.Smart.Ddd.Domain;

internal class ShopMigrationsDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
{
    public ShopDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=localhost;Port=3306;Database=smart;Uid=root;Pwd=123456;";
        var builder = new DbContextOptionsBuilder<ShopDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new ShopDbContext(builder.Options);
    }
}
