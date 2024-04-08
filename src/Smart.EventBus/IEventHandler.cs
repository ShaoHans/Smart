namespace Smart.EventBus;

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);

    Task IEventHandler.HandleAsync(IEvent @event, CancellationToken cancellationToken) => HandleAsync((TEvent)@event, cancellationToken);
}

public interface IEventHandler
{
    Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default);
}
