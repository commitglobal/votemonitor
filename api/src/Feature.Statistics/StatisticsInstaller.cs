using Feature.Statistics.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Statistics;

public static class StatisticsInstaller
{
    public const string SectionKey = "Statistics";

    public static IServiceCollection AddStatisticsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StatisticsFeatureOptions>(configuration);
        return services;
    }
}
