using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.CSOAdmin;

public static class CSOAdminFeatureInstaller
{
    public static IServiceCollection AddCSOAdminFeature(this IServiceCollection services)
    {
        return services;
    }
}
