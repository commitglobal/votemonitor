using Microsoft.Extensions.DependencyInjection;

namespace Feature.Dashboards;

public static class DashboardsFeatureInstaller
{
    public static IServiceCollection AddDashboardsFeature(this IServiceCollection services)
    {
        return services;
    }
}
