using Smart.EventBus;

namespace Sample.Smart.EventBus.Local;

public class UserLoginEvent : EventBase
{
    public string? UserName { get; set; }
}
