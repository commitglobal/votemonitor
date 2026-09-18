using Feature.Statistics.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics;

public static class StatisticsInstaller
{
    public const string SectionKey = "Statistics";
    public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan ObserverCacheDuration = TimeSpan.FromMinutes(120);

    public static IServiceCollection AddStatisticsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StatisticsFeatureOptions>(configuration);

        services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = DefaultCacheDuration;
                // Soft refresh near expiry + fail-safe reduce stampedes and keep serving data under load.
                options.EagerRefreshThreshold = 0.8f;
                options.IsFailSafeEnabled = true;
                options.FailSafeMaxDuration = TimeSpan.FromHours(2);
                options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
            });

        return services;
    }
}
