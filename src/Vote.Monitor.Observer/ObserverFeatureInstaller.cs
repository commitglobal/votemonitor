using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Observer;

public static class ObserverFeatureInstaller
{
    public static IServiceCollection AddObserverFeature(this IServiceCollection services)
    {
        return services;
    }
}
