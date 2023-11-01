using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Feature.PollingStation;
public static class PollingStationFeatureInstaller
{
    public static IServiceCollection AddPollingStationFeature(this IServiceCollection services)
    {
        return services;
    }
}
