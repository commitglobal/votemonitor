using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.Ngo;

public static class NgoFeatureInstaller
{
    public static IServiceCollection AddNgoFeature(this IServiceCollection services)
    {
        return services;
    }
}
