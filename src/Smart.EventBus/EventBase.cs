namespace Smart.EventBus;

public abstract class EventBase : IEvent
{
    public Guid Id { get; init; }
    public DateTime CreationTime { get; init; }

    public EventBase()
    {
        Id = Guid.NewGuid();
        CreationTime = DateTime.Now;
    }
}
