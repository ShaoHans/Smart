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
        builder.Services.Configure<RabbitMQClientSettings>(
            o =>
            {
                o.ConnectionString = settings.ConnectionString;
                o.MaxConnectRetryCount = settings.MaxConnectRetryCount;
                o.ExchangeName = settings.ExchangeName;
                o.ExchangeType = settings.ExchangeType;
                o.QueueName = settings.QueueName;
            }
        );

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
        Action<IConnectionFactory>? configureConnectionFactory = null,
        params Assembly[] eventAssemblies
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

        if (eventAssemblies.Length == 0)
        {
            eventAssemblies = [Assembly.GetEntryAssembly()!];
        }

        var metaDatas = new Dictionary<string, EventMetaData>();
        var settings = new RabbitMQClientSettings
        {
            ExchangeName = builder.Configuration.GetValue<string>($"{configSectionName}:ExchangeName"),
            QueueName = builder.Configuration.GetValue<string>($"{configSectionName}:QueueName"),
            ExchangeType = builder.Configuration.GetValue<string>($"{configSectionName}:ExchangeType")
        };


        foreach (var assembly in eventAssemblies)
        {
            var eventTypes = assembly.GetTypes().Where(t => typeof(IEvent).IsAssignableFrom(t));

            foreach (var eventType in eventTypes) 
            {
                metaDatas.TryAdd(eventType.FullName!, ParseEvent(eventType, settings!));
            }
        }

        builder.Services.Configure<EventMetaDataProvider>(o =>
        {
            o.MetaDatas = metaDatas;
        });

        return builder;
    }

    public static IHostApplicationBuilder AddRabbitMQSubscription(
        this IHostApplicationBuilder builder,
        string configSectionName = "RabbitMQ",
        Action<IConnectionFactory>? configureConnectionFactory = null,
        params Assembly[] eventHandlerAssemblies
    )
    {
        builder.AddRabbitMQ(configSectionName, configureConnectionFactory);
        builder.Services.AddHostedService<RabbitMQSubscriptionHostedService>();

        if (eventHandlerAssemblies.Length == 0)
        {
            eventHandlerAssemblies = [Assembly.GetEntryAssembly()!];
        }

        var eventTypes = new Dictionary<string, Type>();
        foreach (var assembly in eventHandlerAssemblies)
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

    private static EventMetaData ParseEvent(Type eventType, RabbitMQClientSettings settings)
    {
        var attribute = eventType.GetCustomAttribute<EventAttribute>();
        var metaData = new EventMetaData { EventType = eventType };

        if (attribute is null)
        {
            metaData.RouteKey = eventType.FullName;
            metaData.QueueName = settings.QueueName;
            metaData.ExchangeName = settings.ExchangeName;
            metaData.ExchangeType = settings.ExchangeType;
        }
        else 
        {
            metaData.RouteKey = attribute.RouteKey;
            metaData.QueueName = attribute.QueueName;
            metaData.ExchangeName = attribute.ExchangeName;
            metaData.ExchangeType = attribute.ExchangeType;
        }

        return metaData;
    }
}
