using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace Smart.EventBus.RabbitMQ;

internal class RabbitMQSubscriptionHostedService(
    IServiceProvider serviceProvider,
    ILogger<RabbitMQSubscriptionHostedService> logger,
    IConnection connection,
    IOptionsMonitor<RabbitMQClientSettings> mqClientSettingOptions,
    IOptions<RoutingKeyOptions> routingKeyOptions
) : IHostedService
{
    private readonly RabbitMQClientSettings _settings = mqClientSettingOptions.CurrentValue;
    private readonly IModel _channel = connection.CreateModel();
    private readonly Dictionary<string, Type> _eventTypes = routingKeyOptions.Value.EventTypes;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channel.CallbackException += (sender, ea) =>
        {
            logger.LogError(ea.Exception, "RabbitMQ consumer channel occur error");
        };

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceivedAsync;

        _channel.ExchangeDeclare(exchange: _settings.ExchangeName, type: _settings.ExchangeType);

        _channel.QueueDeclare(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

        foreach (var item in _eventTypes)
        {
            _channel.QueueBind(
                queue: _settings.QueueName,
                exchange: _settings.ExchangeName,
                routingKey: item.Key
            );
            logger.LogInformation(
                "bind {@Key} to queue {@QueueName}",
                item.Key,
                _settings.QueueName
            );
        }

        return Task.CompletedTask;
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var eventName = @event.RoutingKey;
        var message = Encoding.UTF8.GetString(@event.Body.Span);
        logger.LogInformation(
            "received rabbitmq message:{@message}, RoutingKey:{@eventName}",
            message,
            eventName
        );

        var eventType = _eventTypes.GetValueOrDefault(eventName);
        if (eventType is null)
        {
            logger.LogWarning("not found RoutingKey [{@eventName}] corresponding Type", eventName);
            return;
        }

        var eventData = JsonSerializer.Deserialize(message, eventType) as EventBase;

        await using var scope = serviceProvider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetKeyedServices<IRabbitMQEventHandler>(eventName);
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
