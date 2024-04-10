namespace Smart.EventBus.RabbitMQ;

public sealed class RabbitMQClientSettings
{
    public string? ConnectionString { get; set; }

    public int MaxConnectRetryCount { get; set; } = 3;

    public string? ExchangeName { get; set; }

    public string? ExchangeType { get; set; }

    public string? QueueName { get; set; }
}
