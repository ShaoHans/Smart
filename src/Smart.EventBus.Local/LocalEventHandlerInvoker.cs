using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Smart.EventBus.Local;

internal class LocalEventHandlerInvoker(
    IServiceProvider serviceProvider,
    ILogger<LocalEventHandlerInvoker> logger
)
{
    public async Task InvokeAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default
    )
        where TEvent : IEvent
    {
        var eventName = @event.GetType().Name;
        var handlers = serviceProvider.GetKeyedServices<ILocalEventHandler>(eventName);
        if (!handlers.Any())
        {
            logger.LogWarning("It has no any event handler for [{@eventName}] event", eventName);
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event, cancellationToken);
        }
    }
}
