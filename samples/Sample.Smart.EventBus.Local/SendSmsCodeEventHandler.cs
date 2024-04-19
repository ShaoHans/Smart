using Microsoft.Extensions.Logging;

using Sample.Smart.EventBus.Message;

using Smart.EventBus.Local;

namespace Sample.Smart.EventBus.Local;

internal class SendSmsCodeEventHandler(ILogger<SendSmsCodeEventHandler> logger) : InProcessEventHandler<UserRegistEvent>(logger)
{
    public override Task InnerHandlerAsync(UserRegistEvent eventData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("已给用户发送手机验证码：{@eventData}", eventData);
        return Task.CompletedTask;
    }
}
