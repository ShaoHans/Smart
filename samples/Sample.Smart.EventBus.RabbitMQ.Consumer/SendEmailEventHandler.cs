using Microsoft.Extensions.Logging;

using Sample.Smart.EventBus.Message;

using Smart.EventBus.RabbitMQ;

namespace Sample.Smart.EventBus.RabbitMQ.Consumer;

internal class SendEmailEventHandler(ILogger<SendEmailEventHandler> logger) : RabbitMQEventHandlerBase<UserRegistEvent>(logger)
{
    public override Task InnerHandlerAsync(UserRegistEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("send email to user {@UserName}", @event.UserName);
        return Task.CompletedTask;
    }
}
