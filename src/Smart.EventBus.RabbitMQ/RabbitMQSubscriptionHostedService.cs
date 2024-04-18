using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Smart.EventBus.RabbitMQ;

internal class RabbitMQSubscriptionHostedService(
    IServiceProvider serviceProvider,
    ILogger<RabbitMQSubscriptionHostedService> logger,
    IConnection connection
) : IHostedService
{
    private readonly IModel _channel = connection.CreateModel();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channel.CallbackException += (sender, ea) =>
        {
            logger.LogError(ea.Exception, "RabbitMQ consumer channel occur error");
        };

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceivedAsync;

        foreach (var (name, type) in EventMetaDataProvider.GroupExchange())
        {
            _channel.ExchangeDeclare(exchange: name, type: type);
        }

        foreach (var queue in EventMetaDataProvider.GroupQueue())
        {
            _channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);
            _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
        }

        foreach (var item in EventMetaDataProvider.MetaDatas)
        {
            _channel.QueueBind(
                queue: item.Value.QueueName,
                exchange: item.Value.ExchangeName,
                routingKey: item.Key
            );
            logger.LogInformation(
                "bind {@Key} to queue {@QueueName}",
                item.Key,
                item.Value.QueueName
            );
        }

        return Task.CompletedTask;
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var message = Encoding.UTF8.GetString(@event.Body.Span);
        logger.LogInformation(
            "received rabbitmq message:{@message}, RoutingKey:{@RoutingKey}",
            message,
            @event.RoutingKey
        );

        var metaData = EventMetaDataProvider.MetaDatas.GetValueOrDefault(@event.RoutingKey);
        if (metaData is null)
        {
            logger.LogWarning(
                "not found RoutingKey [{@RoutingKey}] corresponding event type",
                @event.RoutingKey
            );
            return;
        }

        var eventData = JsonSerializer.Deserialize(message, metaData.EventType!) as EventBase;

        await using var scope = serviceProvider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetKeyedServices<IRabbitMQEventHandler>(
            metaData.EventType!.FullName
        );
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(eventData!);
        }

        _channel.BasicAck(@event.DeliveryTag, false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        connection.Close();
        connection.Dispose();
        return Task.CompletedTask;
    }
}
