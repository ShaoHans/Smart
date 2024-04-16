using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.AddRabbitMQSubscription();

var host = builder.Build();

await host.RunAsync();