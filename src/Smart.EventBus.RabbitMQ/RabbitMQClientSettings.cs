namespace Smart.EventBus.RabbitMQ;

public sealed class RabbitMQClientSettings
{
    public string? ConnectionString { get; set; }

    public int MaxConnectRetryCount { get; set; } = 3;
}
