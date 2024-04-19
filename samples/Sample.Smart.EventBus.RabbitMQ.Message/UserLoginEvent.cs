using Smart.EventBus;

namespace Sample.Smart.EventBus.RabbitMQ.Message;

public class UserLoginEvent : EventBase
{
    public string? UserName { get; set; }
}
