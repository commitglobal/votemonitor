using Microsoft.Extensions.DependencyInjection;

namespace Feature.DataCleanup;

public static class DataCleanupFeatureInstaller
{
    public static IServiceCollection AddDataCleanupFeature(this IServiceCollection services)
    {
        return services;
    }
}
