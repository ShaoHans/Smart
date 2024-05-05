using System.Collections.Concurrent;

namespace Smart.EventBus.Channel;

internal class EventChannelProvider
{
    private static readonly ConcurrentDictionary<string, Channels.Channel<IEvent>> _channels = new();

    public static Channels.Channel<IEvent> GetOrAdd(IEvent _)
    {
        return _channels.GetOrAdd(typeof(IEvent).FullName!, key =>
        {
            var channel = Channels.Channel.CreateUnbounded<IEvent>();
            _channels[key] = channel;
            return channel;
        });
    }
}
