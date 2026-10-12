using Microsoft.Extensions.DependencyInjection;

namespace Feature.PollingStationCorrections;

public static class PollingStationCorrectionsFeatureInstaller
{
    public static IServiceCollection AddPollingStationCorrectionsFeature(this IServiceCollection services)
    {
        return services;
    }
}
