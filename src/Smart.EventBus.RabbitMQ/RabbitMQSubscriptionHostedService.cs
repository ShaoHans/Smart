using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Smart.EventBus.RabbitMQ;

public class RabbitMQSubscriptionHostedService(
    ILogger<RabbitMQSubscriptionHostedService> logger, 
    IConnection connection,
    IOptionsMonitor<RabbitMQClientSettings> options) : IHostedService
{
    private readonly RabbitMQClientSettings _settings = options.CurrentValue;
    private readonly IModel _channel = connection.CreateModel();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channel.CallbackException += (sender, ea) =>
        {
            logger.LogWarning(ea.Exception, "RabbitMQ consumer channel occur error");
        };

        _channel.ExchangeDeclare(
            exchange: _settings.ExchangeName,
            type: _settings.ExchangeType);

        _channel.QueueDeclare(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;

        _channel.BasicConsume(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer
            );

        _channel.QueueBind(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: ""
            );

        return Task.CompletedTask;
    }

    private Task OnMessageReceived(object sender, BasicDeliverEventArgs @event)
    {
        _channel.BasicAck(@event.DeliveryTag, false);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        connection.Close();
        connection.Dispose();
        return Task.CompletedTask;
    }
}
