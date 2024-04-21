using Sample.Smart.Ddd.Domain.Models;
using Smart.Ddd.Domain.EntityFrameworkCore.Repositories;
using Smart.Ddd.Domain.Uow;

namespace Sample.Smart.Ddd.Domain.Repositories;

internal class UserRepository(ShopDbContext context, IUnitOfWork unitOfWork)
    : EfCoreRepository<ShopDbContext, User, int>(context, unitOfWork),
        IUserRepository { }
