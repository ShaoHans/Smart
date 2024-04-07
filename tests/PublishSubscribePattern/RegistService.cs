namespace PublishSubscribePattern;

/// <summary>
/// 注册服务（被观察者）
/// </summary>
internal class RegistService
{
    public delegate void RegistHandler(User user);
    public event RegistHandler? RegistEvent;

    public void Regist(User user)
    {
        Console.WriteLine($"新用户注册了：{user}");

        RegistEvent?.Invoke(user);
    }
}
