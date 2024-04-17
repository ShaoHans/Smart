namespace Sample.PublishSubscribePattern;

/// <summary>
/// 邮件服务（观察者）
/// </summary>
internal class EmailService
{
    public void Send(User user)
    {
        Console.WriteLine($"已经给{user.Name}的邮箱{user.Email}发送了激活邮件");
    }
}
