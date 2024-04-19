using Microsoft.Extensions.Logging;

using Smart.EventBus.Local;

namespace Sample.Smart.EventBus.Local;

internal class SendEmailEventHandler(ILogger<SendEmailEventHandler> logger) : LocalEventHandler<UserRegistEvent>(logger)
{
    public override Task InnerHandlerAsync(UserRegistEvent eventData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("已给用户发送激活邮件：{@eventData}", eventData);
        return Task.CompletedTask;
    }
}
