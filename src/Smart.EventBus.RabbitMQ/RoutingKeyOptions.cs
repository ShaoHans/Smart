namespace Smart.EventBus.RabbitMQ;

internal class RoutingKeyOptions
{
    public Dictionary<string, Type> EventTypes { get; set; } = [];
}
