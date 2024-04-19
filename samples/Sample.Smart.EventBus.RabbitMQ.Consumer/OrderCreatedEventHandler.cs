using Microsoft.Extensions.Logging;

using Sample.Smart.EventBus.RabbitMQ.Message;

using Smart.EventBus.RabbitMQ;

namespace Sample.Smart.EventBus.RabbitMQ.Consumer;

internal class OrderCreatedEventHandler : RabbitMQEventHandlerBase<OrderCreatedEvent>
{
    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger) : base(logger)
    {
    }

    public override Task InnerHandlerAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("order created, add score to user {@UserName}", @event.UserName);
        return Task.CompletedTask;
    }
}
