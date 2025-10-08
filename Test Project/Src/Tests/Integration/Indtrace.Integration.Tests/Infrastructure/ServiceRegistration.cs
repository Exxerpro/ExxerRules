using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Integration.Tests.Infrastructure;

public static class ServiceRegistration
{


    public static T GetRequiredKeyedService<T>(this IServiceProvider services, string key)
        where T : notnull
    {
        var svc = services.GetKeyedService<T>(key);
        return svc is null
            ? throw new InvalidOperationException($"No service for type '{typeof(T)}' and key '{key}' has been registered.")
            : svc;
    }
}
