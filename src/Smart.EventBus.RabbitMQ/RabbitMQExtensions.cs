using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Polly;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

using Smart.EventBus;
using Smart.EventBus.RabbitMQ;

using System.Net.Sockets;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMQExtensions
{
    public static IHostApplicationBuilder AddRabbitMQEventBus(this IHostApplicationBuilder builder,
        string configSectionName = "RabbitMQ",
        string serviceKey = "",
        Action<IConnectionFactory>? configureConnectionFactory = null)
    {
        builder.AddRabbitMQ(configSectionName, configureConnectionFactory);

        if (string.IsNullOrEmpty(serviceKey))
        {
            builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
        }
        else
        {
            builder.Services.AddKeyedSingleton<IEventBus, RabbitMQEventBus>(serviceKey);
        }
        return builder;
    }

    public static void AddRabbitMQ(this IHostApplicationBuilder builder, 
        string configSectionName = "RabbitMQ", 
        Action<IConnectionFactory>? configureConnectionFactory = null)
    {
        var configSection = builder.Configuration.GetSection(configSectionName);
        var settings = new RabbitMQClientSettings();
        configSection.Bind(settings);

        if (string.IsNullOrEmpty(settings.ConnectionString))
        {
            throw new ArgumentNullException("RabbitMQ connection string cannot be null, please set it on your appsetting.json file, like : [RabbitMQ:ConnectionString]");
        }

        IConnectionFactory CreateConnectionFactory(IServiceProvider sp)
        {
            var factory = new ConnectionFactory
            {
                Uri = new(settings.ConnectionString!)
            };
            configureConnectionFactory?.Invoke(factory);
            return factory;
        }

        builder.Services.AddSingleton(CreateConnectionFactory);
        builder.Services.AddSingleton(sp => CreateConnection(sp.GetRequiredService<IConnectionFactory>(), settings.MaxConnectRetryCount));
    }


    private static IConnection CreateConnection(IConnectionFactory factory, int retryCount)
    {
        var policy = Policy
            .Handle<SocketException>().Or<BrokerUnreachableException>()
            .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        return policy.Execute(() =>
        {
            return factory.CreateConnection();
        });
    }

}
