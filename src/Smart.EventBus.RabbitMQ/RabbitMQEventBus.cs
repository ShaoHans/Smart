using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

using System.Text.Json;

namespace Smart.EventBus.RabbitMQ;

internal class RabbitMQEventBus(
    ILogger<RabbitMQEventBus> logger, 
    IConnection connection, 
    IOptionsMonitor<RabbitMQClientSettings> options) : IEventBus
{
    private readonly RabbitMQClientSettings _settings = options.CurrentValue;

    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        using var channel = connection.CreateModel();
        var routingKey = @event.GetType().Name;

        channel.ExchangeDeclare(exchange: _settings.ExchangeName, _settings.ExchangeType);
        var properties = channel.CreateBasicProperties();
        properties.MessageId = @event.Id.ToString();
        properties.DeliveryMode = 2;
        channel.BasicPublish(
            exchange: _settings.ExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType()));
        return Task.CompletedTask;
    }
}
