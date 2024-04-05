using Microsoft.Extensions.DependencyInjection;

namespace Feature.MonitoringObservers;

public static class MonitoringObserverFeatureInstaller
{
    public static IServiceCollection AddMonitoringObserversFeature(this IServiceCollection services)
    {
        return services;
    }
}
