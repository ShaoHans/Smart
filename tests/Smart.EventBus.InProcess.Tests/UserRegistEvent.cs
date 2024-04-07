namespace Smart.EventBus.InProcess.Tests;

internal class UserRegistEvent : EventBase
{
    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }
}
