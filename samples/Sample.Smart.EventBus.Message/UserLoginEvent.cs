using Smart.EventBus;

namespace Sample.Smart.EventBus.Message;

public class UserLoginEvent : EventBase
{
    public string? UserName { get; set; }
}
