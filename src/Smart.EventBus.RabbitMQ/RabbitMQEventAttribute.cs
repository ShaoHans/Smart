namespace Smart.EventBus.RabbitMQ;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RabbitMQEventAttribute : Attribute
{
    public string ExchangeName { get; set; } = "default";

    public string ExchangeType { get; set; } = "direct";

    public string RoutingKey { get; set; } = "*";

    public string QueueName { get; set; } = "";
}
