using Microsoft.Extensions.Logging;

namespace Smart.EventBus.InProcess;

public abstract class InProcessEventHandler<TEvent>(ILogger<InProcessEventHandler<TEvent>> logger) : EventHandlerBase<TEvent>(logger) where TEvent : IEvent
{
}
