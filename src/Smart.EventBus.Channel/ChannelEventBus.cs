namespace Smart.EventBus.Channel;

internal class ChannelEventBus : IEventBus
{
    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var channel = EventChannelProvider.GetOrAdd(@event);
        await channel.Writer.WriteAsync(@event, cancellationToken);
    }
}
