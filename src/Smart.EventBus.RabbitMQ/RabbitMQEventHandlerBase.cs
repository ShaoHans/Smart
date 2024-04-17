using Microsoft.Extensions.Logging;

namespace Smart.EventBus.RabbitMQ;

public abstract class RabbitMQEventHandlerBase<TEvent>(
    ILogger<RabbitMQEventHandlerBase<TEvent>> logger
) : EventHandlerBase<TEvent>(logger), IRabbitMQEventHandler
    where TEvent : IEvent { }
