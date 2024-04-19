using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Sample.Smart.EventBus.RabbitMQ.Message;

using Smart.EventBus;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLogging(configure => configure.AddConsole());
builder.AddRabbitMQEventBus(eventAssemblies: typeof(UserLoginEvent).Assembly);

var host = builder.Build();

var eventBus = host.Services.GetRequiredService<IEventBus>();
await eventBus.PublishAsync(new UserRegistEvent { UserName = "tom", Mobile = "110", Email = "tom@gmail.com" });
Console.WriteLine("has sent user registed event message");

await eventBus.PublishAsync(new OrderCreatedEvent { OrderId = Guid.NewGuid().ToString(), UserName = "jim", OrderDate = DateTime.Now });
Console.WriteLine("has sent order created event message");

await host.RunAsync();