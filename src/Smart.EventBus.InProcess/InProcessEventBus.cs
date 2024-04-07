using Microsoft.Extensions.DependencyInjection;

namespace Smart.EventBus.InProcess;

public class InProcessEventBus(IServiceProvider serviceProvider) : IEventBus
{
    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var handlers = serviceProvider.GetKeyedServices<InProcessEventHandler<IEvent>>("InProcess_EventHandler");
        if (!handlers.Any())
        {
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event, cancellationToken);
        }
    }
}
