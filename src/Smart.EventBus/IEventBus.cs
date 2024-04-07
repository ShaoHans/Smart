namespace Smart.EventBus;

public interface IEventBus
{
    Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);
}
