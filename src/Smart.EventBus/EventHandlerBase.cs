using Microsoft.Extensions.Logging;

namespace Smart.EventBus;

public abstract class EventHandlerBase<TEvent>(ILogger<EventHandlerBase<TEvent>> logger)
    : IEventHandler<TEvent>
    where TEvent : IEvent
{
    protected readonly ILogger _logger = logger;

    public virtual Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            return InnerHandlerAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            return HandleExceptionAsync(ex, @event);
        }
    }

    public abstract Task InnerHandlerAsync(
        TEvent @event,
        CancellationToken cancellationToken = default
    );

    public virtual Task HandleExceptionAsync(Exception exception, TEvent eventData)
    {
        _logger.LogError(exception, "handle event occurs an exception : {@eventData}", eventData);
        return Task.CompletedTask;
    }
}
