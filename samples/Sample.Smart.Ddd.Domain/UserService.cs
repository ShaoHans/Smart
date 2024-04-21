using Sample.Smart.Ddd.Domain.Models;
using Sample.Smart.Ddd.Domain.Repositories;
using Smart.Ddd.Domain.Repositories;
using Smart.Ddd.Domain.Uow;

namespace Sample.Smart.Ddd.Domain;

internal class UserService(
    IUnitOfWork uow,
    IRepository<User> userRepository,
    IUserRepository userRepository2
)
{
    public async Task AddAsync(string userName)
    {
        if (await userRepository.GetCountAsync(u => u.UserName == userName) > 0)
        {
            Console.WriteLine($"has exist the same username:{userName}");
            return;
        }

        var user = new User
        {
            UUId = Guid.NewGuid(),
            UserName = userName,
            RegistTime = DateTime.Now,
        };
        await userRepository.AddAsync(user);
        await uow.SaveChangesAsync();
    }

    public async Task<User?> GetAsync(int id)
    {
        return await userRepository2.FindAsync(id);
    }
}
