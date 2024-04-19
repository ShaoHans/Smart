using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Smart.EventBus;
using Smart.EventBus.Local;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInProcessEventBus(
        this IServiceCollection services,
        string serviceKey = "",
        params Assembly[] assemblies
    )
    {
        if (string.IsNullOrEmpty(serviceKey))
        {
            services.TryAddSingleton<IEventBus, LocalEventBus>();
        }
        else
        {
            services.TryAddKeyedSingleton<IEventBus, LocalEventBus>(serviceKey);
        }

        services.AddSingleton<LocalEventHandlerInvoker>();

        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetEntryAssembly()!];
        }

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly
                .GetTypes()
                .Where(t => typeof(ILocalEventHandler).IsAssignableFrom(t));
            foreach (var handlerType in handlerTypes)
            {
                services.AddKeyedTransient(
                    typeof(ILocalEventHandler),
                    handlerType.BaseType!.GetGenericArguments()[0].Name,
                    handlerType
                );
            }
        }

        return services;
    }
}
