namespace Smart.EventBus;

public interface IEvent
{
    public Guid EventId { get; }

    public DateTime CreationTime { get; }
}
