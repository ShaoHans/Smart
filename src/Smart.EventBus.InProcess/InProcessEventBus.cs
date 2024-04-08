namespace Smart.EventBus.InProcess;

public class InProcessEventBus(InProcessEventHandlerInvoker invoker) : IEventBus
{
    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        await invoker.InvokeAsync(@event, cancellationToken);
    }
}
