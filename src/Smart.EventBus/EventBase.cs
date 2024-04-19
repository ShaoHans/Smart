namespace Smart.EventBus;

public abstract class EventBase : IEvent
{
    public Guid EventId { get; init; }
    public DateTime CreationTime { get; init; }

    public EventBase()
    {
        EventId = Guid.NewGuid();
        CreationTime = DateTime.Now;
    }
}
