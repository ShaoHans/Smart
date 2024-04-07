namespace Smart.EventBus;

public interface IEvent
{
    public Guid Id { get; }

    public DateTime CreationTime { get; }
}
