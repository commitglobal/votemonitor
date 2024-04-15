using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.NgoAdmin;

public static class NgoAdminFeatureInstaller
{
    public static IServiceCollection AddNgoAdminFeature(this IServiceCollection services)
    {
        return services;
    }
}
