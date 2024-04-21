using Microsoft.EntityFrameworkCore;
using Smart.Data;

namespace Smart.Ddd.Domain.EntityFrameworkCore;

public abstract class SmartEfCoreDbContext<TDbContext>(DbContextOptions<TDbContext> options)
    : DbContext(options),
        ISmartDbContext
    where TDbContext : DbContext { }
