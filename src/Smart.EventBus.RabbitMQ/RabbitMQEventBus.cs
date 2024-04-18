﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Smart.EventBus.RabbitMQ;

internal class RabbitMQEventBus(ILogger<RabbitMQEventBus> logger, IConnection connection)
    : IEventBus
{
    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        using var channel = connection.CreateModel();
        var name = @event.GetType().FullName;
        var metaData =
            EventMetaDataProvider.MetaDatas!.GetValueOrDefault(name)
            ?? throw new ArgumentNullException($"not found the [{name}] event meta data ");

        channel.ExchangeDeclare(exchange: metaData.ExchangeName, metaData.ExchangeType);
        var properties = channel.CreateBasicProperties();
        properties.MessageId = @event.Id.ToString();
        properties.DeliveryMode = 2;
        channel.BasicPublish(
            exchange: metaData.ExchangeName,
            routingKey: metaData.RouteKey,
            mandatory: true,
            basicProperties: properties,
            body: JsonSerializer.SerializeToUtf8Bytes(@event, metaData.EventType!)
        );
        return Task.CompletedTask;
    }
}
