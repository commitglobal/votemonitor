using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.ElectionRound;

public static class ElectionRoundFeatureInstaller
{
    public static IServiceCollection AddElectionRoundFeature(this IServiceCollection services)
    {
        return services;
    }
}
