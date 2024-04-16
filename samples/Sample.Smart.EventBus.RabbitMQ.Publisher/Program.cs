using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sample.Smart.EventBus.Message;

using Smart.EventBus;

var builder = Host.CreateApplicationBuilder(args);
builder.AddRabbitMQEventBus();

var host = builder.Build();

var eventBus = host.Services.GetRequiredService<IEventBus>();
await eventBus.PublishAsync(new UserRegistEvent { UserName = "tom", Mobile = "110", Email = "tom@gmail.com" });
await host.RunAsync();