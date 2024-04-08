using Microsoft.Extensions.Logging;

namespace Smart.EventBus.InProcess.Tests;

internal class AddScoreEventHandler(ILogger<AddScoreEventHandler> logger) : InProcessEventHandler<UserLoginEvent>(logger)
{
    public override Task InnerHandlerAsync(UserLoginEvent eventData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("用户：{@eventData}登录了，加10个积分", eventData);
        return Task.CompletedTask;
    }
}
