using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Smart.Ddd.Domain;

var builder = Host.CreateApplicationBuilder();
builder.Services.AddSmartDbContext<ShopDbContext>("Server=localhost;Port=3306;Database=smart;Uid=root;Pwd=123456;");
var host = builder.Build();
await host.RunAsync();
