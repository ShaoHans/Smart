using Microsoft.Extensions.Logging;

namespace Smart.EventBus.RabbitMQ;

public abstract class RabbitMQEventHandler<TEvent>(ILogger<RabbitMQEventHandler<TEvent>> logger) 
    : EventHandlerBase<TEvent>(logger), 
        IRabbitMQEventHandler where TEvent : IEvent
{
    
}
