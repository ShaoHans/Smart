using Sample.Smart.Ddd.Domain.Models;

using Smart.Ddd.Domain.Repositories;

namespace Sample.Smart.Ddd.Domain.Repositories;

internal interface IUserRepository : IRepository<User, int>
{
}
