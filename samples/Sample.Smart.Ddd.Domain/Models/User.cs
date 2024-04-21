using Smart.Ddd.Domain.Entities;

namespace Sample.Smart.Ddd.Domain.Models;

internal class User : IEntity<int>
{
    public int Id { get; set; }

    public Guid UUId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public DateTime RegistTime { get; set; }

    public object[]? GetKeys()
    {
        return [Id];
    }

}
