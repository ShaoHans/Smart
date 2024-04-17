using System.Net.Sockets;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Smart.EventBus;
using Smart.EventBus.RabbitMQ;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMQExtensions
{
    public static IHostApplicationBuilder AddRabbitMQ(
        this IHostApplicationBuilder builder,
        string configSectionName = "RabbitMQ",
        Action<IConnectionFactory>? configureConnectionFactory = null
    )
    {
        var configSection = builder.Configuration.GetSection(configSectionName);
        var settings = new RabbitMQClientSettings();
        configSection.Bind(settings);
        builder.Services.Configure<RabbitMQClientSettings>(o =>
        {
            o.ConnectionString = settings.ConnectionString;
            o.MaxConnectRetryCount = settings.MaxConnectRetryCount;
            o.ExchangeName = settings.ExchangeName;
            o.ExchangeType = settings.ExchangeType;
            o.QueueName = settings.QueueName;
        });

        if (string.IsNullOrEmpty(settings.ConnectionString))
        {
            throw new ArgumentNullException(
                "RabbitMQ connection string cannot be null, please set it on your appsetting.json file, like : [RabbitMQ:ConnectionString]"
            );
        }

        IConnectionFactory CreateConnectionFactory(IServiceProvider sp)
        {
            var factory = new ConnectionFactory
            {
                Uri = new(settings.ConnectionString!),
                DispatchConsumersAsync = true
            };
            configureConnectionFactory?.Invoke(factory);
            return factory;
        }

        builder.Services.AddSingleton(CreateConnectionFactory);
        builder.Services.AddSingleton(sp =>
            CreateConnection(
                sp.GetRequiredService<IConnectionFactory>(),
                settings.MaxConnectRetryCount
            )
        );

        return builder;
    }

    public static IHostApplicationBuilder AddRabbitMQEventBus(
        this IHostApplicationBuilder builder,
        string configSectionName = "RabbitMQ",
        string serviceKey = "",
        Action<IConnectionFactory>? configureConnectionFactory = null
    )
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

    public static IHostApplicationBuilder AddRabbitMQSubscription(
        this IHostApplicationBuilder builder,
        string configSectionName = "RabbitMQ",
        Action<IConnectionFactory>? configureConnectionFactory = null,
        params Assembly[] handlerAssemblies
    )
    {
        builder.AddRabbitMQ(configSectionName, configureConnectionFactory);
        builder.Services.AddHostedService<RabbitMQSubscriptionHostedService>();

        if (handlerAssemblies.Length == 0)
        {
            handlerAssemblies = [Assembly.GetEntryAssembly()!];
        }

        var eventTypes = new Dictionary<string, Type>();
        foreach (var assembly in handlerAssemblies)
        {
            var handlerTypes = assembly
                .GetTypes()
                .Where(t => typeof(IRabbitMQEventHandler).IsAssignableFrom(t));
            foreach (var handlerType in handlerTypes)
            {
                var routingKey = handlerType.BaseType!.GenericTypeArguments[0].FullName;
                eventTypes.TryAdd(routingKey!, handlerType.BaseType!.GenericTypeArguments[0]);
                builder.Services.AddKeyedTransient(
                    typeof(IRabbitMQEventHandler),
                    routingKey,
                    handlerType
                );
            }
        }

        builder.Services.Configure<RoutingKeyOptions>(o =>
        {
            o.EventTypes = eventTypes;
        });

        return builder;
    }

    private static IConnection CreateConnection(IConnectionFactory factory, int retryCount)
    {
        var policy = Policy
            .Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );

        return policy.Execute(() =>
        {
            return factory.CreateConnection();
        });
    }
}
