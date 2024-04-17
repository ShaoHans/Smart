namespace Smart.EventBus.RabbitMQ;

public class RabbitMQEvent : EventBase
{
    public Dictionary<string, object> Headers { get; set; } = [];
}
