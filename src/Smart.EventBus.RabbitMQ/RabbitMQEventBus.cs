﻿using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using System.Text.Json;

namespace Smart.EventBus.RabbitMQ;

internal class RabbitMQEventBus(ILogger<RabbitMQEventBus> logger, IConnection connection) : IEventBus
{
    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        using var channel = connection.CreateModel();
        var exchange = "Smart";
        var routingKey = @event.GetType().Name;

        channel.ExchangeDeclare(exchange: exchange, type: "direct");
        var properties = channel.CreateBasicProperties();
        properties.MessageId = @event.Id.ToString();
        properties.DeliveryMode = 2;
        channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType()));
        return Task.CompletedTask;
    }
}
