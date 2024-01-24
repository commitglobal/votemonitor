using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.Monitoring;

public static class MonitoringFeatureInstaller
{
    public static IServiceCollection AddMonitoringFeature(this IServiceCollection services)
    {
        return services;
    }
}
