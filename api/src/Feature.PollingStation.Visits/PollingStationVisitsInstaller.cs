using Microsoft.Extensions.DependencyInjection;

namespace Feature.PollingStation.Visits;

public static class PollingStationVisitsInstaller
{
    public static IServiceCollection AddPollingStationVisitsFeature(this IServiceCollection services)
    {
        return services;
    }
}
