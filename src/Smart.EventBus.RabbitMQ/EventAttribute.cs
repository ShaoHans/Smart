namespace Smart.EventBus.RabbitMQ;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EventAttribute : Attribute
{
    public string ExchangeName { get; set; } = "default";

    public string ExchangeType { get; set; } = "direct";

    public string RouteKey { get; set; } = "*";

    public string QueueName { get; set; } = "";
}
