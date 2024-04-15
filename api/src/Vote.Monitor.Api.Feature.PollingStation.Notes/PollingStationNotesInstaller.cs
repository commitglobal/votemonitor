using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes;

public static class PollingStationNotesInstaller
{
    public static IServiceCollection AddPollingStationNotesFeature(this IServiceCollection services)
    {
        return services;
    }
}
