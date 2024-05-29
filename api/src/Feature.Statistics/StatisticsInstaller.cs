using Microsoft.Extensions.DependencyInjection;

namespace Feature.Statistics;

public static class StatisticsInstaller
{
    public static IServiceCollection AddStatisticsFeature(this IServiceCollection services)
    {
        return services;
    }
}
