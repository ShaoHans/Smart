using Smart.EventBus;
using Smart.EventBus.RabbitMQ;

namespace Sample.Smart.EventBus.RabbitMQ.Message;

[RabbitMQEvent(ExchangeName = "order", ExchangeType = "topic", RoutingKey = "fruit.order", QueueName = "consumer.test")]
public class OrderCreatedEvent : EventBase
{
    public string? OrderId { get; set; }

    public string? UserName { get; set; }

    public DateTime OrderDate { get; set; }
}
