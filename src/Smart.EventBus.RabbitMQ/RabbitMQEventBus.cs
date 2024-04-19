using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Smart.EventBus.RabbitMQ;

internal class RabbitMQEventBus(ILogger<RabbitMQEventBus> logger, IConnection connection)
    : IEventBus
{
    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        using var channel = connection.CreateModel();
        var metaData = EventMetaDataProvider.GetMetaData(@event.GetType());
        if (string.IsNullOrEmpty(metaData.Key))
        {
            throw new ArgumentException($"not found {@event.GetType()} event meta data");
        }

        channel.ExchangeDeclare(exchange: metaData.Value.ExchangeName, metaData.Value.ExchangeType);
        var properties = channel.CreateBasicProperties();
        properties.MessageId = @event.EventId.ToString();
        properties.DeliveryMode = 2;
        channel.BasicPublish(
            exchange: metaData.Value.ExchangeName,
            routingKey: metaData.Key,
            mandatory: true,
            basicProperties: properties,
            body: JsonSerializer.SerializeToUtf8Bytes(@event, metaData.Value.EventType!)
        );
        return Task.CompletedTask;
    }
}
