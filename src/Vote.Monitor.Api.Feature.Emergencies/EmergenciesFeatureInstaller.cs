using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.Emergencies;

public static class EmergenciesFeatureInstaller
{
    public static IServiceCollection AddEmergenciesFeature(this IServiceCollection services)
    {
        return services;
    }
}
