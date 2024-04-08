using Microsoft.Extensions.DependencyInjection;

namespace Smart.EventBus.InProcess;

public class InProcessEventHandlerInvoker(IServiceProvider serviceProvider)
{
    public async Task InvokeAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var handlers = serviceProvider.GetKeyedServices<InProcessEventHandler<TEvent>>(@event.GetType().Name);
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event, cancellationToken);        
        }
    }
}
