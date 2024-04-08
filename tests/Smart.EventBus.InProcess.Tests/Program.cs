// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smart.EventBus;
using Smart.EventBus.InProcess;
using Smart.EventBus.InProcess.Tests;

var services = new ServiceCollection();
services.AddSingleton<IEventBus, InProcessEventBus>();
services.AddKeyedSingleton(typeof(InProcessEventHandler<UserRegistEvent>), "UserRegistEvent", typeof(SendEmailEventHandler));
services.AddSingleton<InProcessEventHandlerInvoker>();
services.AddLogging(configure => configure.AddConsole());
var provider = services.BuildServiceProvider();

var eventBus = provider.GetRequiredService<IEventBus>();
await eventBus.PublishAsync(new UserRegistEvent { UserName = "shz", Email = "111@qq.com", Mobile = "12345678910" });