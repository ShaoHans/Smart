using Sample.Smart.Ddd.Domain.Models;
using Smart.Ddd.Domain.Repositories;
using Smart.Ddd.Domain.Uow;

namespace Sample.Smart.Ddd.Domain;

internal class UserService(IUnitOfWork uow, IRepository<User> userRepository)
{
    public async Task AddAsync(string userName)
    {
        var user = new User
        {
            UUId = Guid.NewGuid(),
            UserName = userName,
            RegistTime = DateTime.Now,
        };
        await userRepository.AddAsync(user);
        await uow.SaveChangesAsync();
    }
}
