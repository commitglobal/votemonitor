using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.PollingStation.Information;

public static class PollingStationInformationInstaller
{
    public static IServiceCollection AddPollingStationInformationFeature(this IServiceCollection services)
    {
        return services;
    }
}
