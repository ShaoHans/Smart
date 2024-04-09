using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Smart.EventBus.InProcess;

internal class InProcessEventHandlerInvoker(IServiceProvider serviceProvider, ILogger<InProcessEventHandlerInvoker> logger)
{
    public async Task InvokeAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {        
        var eventName = @event.GetType().Name;
        var handlers = serviceProvider.GetKeyedServices<IInProcessEventHandler>(eventName);
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
