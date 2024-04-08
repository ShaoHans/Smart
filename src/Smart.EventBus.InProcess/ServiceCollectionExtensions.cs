using Smart.EventBus;
using Smart.EventBus.InProcess;

using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInProcessEventBus(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<IEventBus, InProcessEventBus>();
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
                services.AddKeyedSingleton(typeof(IInProcessEventHandler), handlerType.BaseType!.GetGenericArguments()[0].Name, handlerType);
            }
        }

        return services;
    }
}
