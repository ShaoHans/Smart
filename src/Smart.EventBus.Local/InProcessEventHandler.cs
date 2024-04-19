using Microsoft.Extensions.Logging;

namespace Smart.EventBus.Local;

public abstract class InProcessEventHandler<TEvent>(ILogger<InProcessEventHandler<TEvent>> logger)
    : EventHandlerBase<TEvent>(logger),
        IInProcessEventHandler
    where TEvent : IEvent { }
