using Microsoft.Extensions.Logging;

namespace Smart.EventBus.InProcess.Tests;

internal class SendSmsCodeEventHandler(ILogger<SendSmsCodeEventHandler> logger) : InProcessEventHandler<UserRegistEvent>(logger)
{
    public override Task InnerHandlerAsync(UserRegistEvent eventData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("已给用户发送手机验证码：{@eventData}", eventData);
        return Task.CompletedTask;
    }
}
