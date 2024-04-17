// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sample.Smart.EventBus.Message;

using Smart.EventBus;

var services = new ServiceCollection();
services.AddInProcessEventBus();
services.AddLogging(configure => configure.AddConsole());
var provider = services.BuildServiceProvider();

var eventBus = provider.GetRequiredService<IEventBus>();
await eventBus.PublishAsync(new UserRegistEvent { UserName = "shz", Email = "111@qq.com", Mobile = "12345678910" });
await eventBus.PublishAsync(new UserLoginEvent { UserName = "shz" });