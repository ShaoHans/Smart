// See https://aka.ms/new-console-template for more information
using Sample.PublishSubscribePattern;

var emailService = new EmailService();
var registService = new RegistService();
registService.RegistEvent += emailService.Send;

for (int i = 0; i < 10; i++)
{
    registService.Regist(new User { Name = $"user{i}", Email = $"user{i}@qq.com" });
}
