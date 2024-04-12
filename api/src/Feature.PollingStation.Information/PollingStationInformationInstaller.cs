using Microsoft.Extensions.DependencyInjection;

namespace Feature.PollingStation.Information;

public static class PollingStationInformationInstaller
{
    public static IServiceCollection AddPollingStationInformationFeature(this IServiceCollection services)
    {
        return services;
    }
}
