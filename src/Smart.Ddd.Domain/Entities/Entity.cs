namespace Smart.Ddd.Domain.Entities;

public abstract class Entity : IEntity
{
    public abstract object[]? GetKeys();
}

public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    public virtual TKey Id { get; protected set; } = default!;
}
