using Microsoft.Extensions.DependencyInjection.Extensions;

using Smart.EventBus;
using Smart.EventBus.InProcess;

using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInProcessEventBus(this IServiceCollection services, string serviceKey = "", params Assembly[] assemblies)
    {
        if (string.IsNullOrEmpty(serviceKey))
        {
            services.TryAddSingleton<IEventBus, InProcessEventBus>();
        }
        else
        {
            services.TryAddKeyedSingleton<IEventBus, InProcessEventBus>(serviceKey);
        }

        services.AddSingleton<InProcessEventHandlerInvoker>();

        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetEntryAssembly()!];
        }

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes().Where(t => typeof(IInProcessEventHandler).IsAssignableFrom(t));
            foreach (var handlerType in handlerTypes)
            {
                services.AddKeyedTransient(typeof(IInProcessEventHandler), handlerType.BaseType!.GetGenericArguments()[0].Name, handlerType);
            }
        }

        return services;
    }
}
