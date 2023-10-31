using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.ElectionRound;

public static class ElectionRoundFeatureInstaller
{
    public static IServiceCollection AddElectionRoundFeature(this IServiceCollection services)
    {
        return services;
    }
}
