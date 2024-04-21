using Microsoft.EntityFrameworkCore;
using Sample.Smart.Ddd.Domain.Models;
using Smart.Ddd.Domain.EntityFrameworkCore;

namespace Sample.Smart.Ddd.Domain;

internal class ShopDbContext(DbContextOptions<ShopDbContext> options)
    : SmartEfCoreDbContext<ShopDbContext>(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(u => u.UUId).IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(u => u.UserName)
            .HasColumnType("varchar(40)")
            .IsRequired();

        modelBuilder.Entity<User>().Property(u => u.RegistTime).IsRequired();
    }
}
