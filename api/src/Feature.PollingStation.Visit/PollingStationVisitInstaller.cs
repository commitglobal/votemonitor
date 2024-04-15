using Microsoft.Extensions.DependencyInjection;

namespace Feature.PollingStation.Visit;

public static class PollingStationVisitInstaller
{
    public static IServiceCollection AddPollingStationVisitFeature(this IServiceCollection services)
    {
        return services;
    }
}
