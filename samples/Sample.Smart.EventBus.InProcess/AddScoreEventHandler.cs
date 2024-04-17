using Microsoft.Extensions.Logging;

using Sample.Smart.EventBus.Message;

using Smart.EventBus.InProcess;

namespace Sample.Smart.EventBus.InProcess;

internal class AddScoreEventHandler(ILogger<AddScoreEventHandler> logger) : InProcessEventHandler<UserLoginEvent>(logger)
{
    public override Task InnerHandlerAsync(UserLoginEvent eventData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("用户：{@eventData}登录了，加10个积分", eventData);
        return Task.CompletedTask;
    }
}
