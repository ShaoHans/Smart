namespace Smart.EventBus.RabbitMQ;

internal class EventMetaData
{
    public Type? EventType { get; set; }

    public string? ExchangeName { get; set; }

    public string? ExchangeType { get; set; }

    public string? QueueName { get; set; }
}
