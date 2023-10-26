using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.CSOAdmin;

public static class CSOAdminFeatureInstaller
{
    public static IServiceCollection AddCSOFeature(this IServiceCollection services)
    {
        return services;
    }
}
