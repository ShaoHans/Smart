using Smart.EventBus;

namespace Sample.Smart.EventBus.Local;

public class UserRegistEvent : EventBase
{
    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }
}