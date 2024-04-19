using Microsoft.Extensions.Logging;

namespace Smart.EventBus.Local;

public abstract class LocalEventHandler<TEvent>(ILogger<LocalEventHandler<TEvent>> logger)
    : EventHandlerBase<TEvent>(logger),
        ILocalEventHandler
    where TEvent : IEvent { }
