﻿namespace Smart.EventBus.Local;

internal class InProcessEventBus(InProcessEventHandlerInvoker invoker) : IEventBus
{
    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        await invoker.InvokeAsync(@event, cancellationToken);
    }
}