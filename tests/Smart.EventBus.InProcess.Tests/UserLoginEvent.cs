namespace Smart.EventBus.InProcess.Tests;

internal class UserLoginEvent : EventBase
{
    public string? UserName { get; set; }
}
