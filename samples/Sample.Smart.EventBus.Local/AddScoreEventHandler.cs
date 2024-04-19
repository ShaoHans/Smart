using Microsoft.Extensions.Logging;

using Smart.EventBus.Local;

namespace Sample.Smart.EventBus.Local;

internal class AddScoreEventHandler(ILogger<AddScoreEventHandler> logger) : LocalEventHandler<UserLoginEvent>(logger)
{
    public override Task InnerHandlerAsync(UserLoginEvent eventData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("用户：{@eventData}登录了，加10个积分", eventData);
        return Task.CompletedTask;
    }
}
