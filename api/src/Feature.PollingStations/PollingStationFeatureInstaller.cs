using Feature.PollingStations.Options;
using Feature.PollingStations.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.PollingStations;
public static class PollingStationFeatureInstaller
{
    public const string SectionKey = "PollingStationsFeatureConfig";

    public static IServiceCollection AddPollingStationFeature(this IServiceCollection services, IConfigurationSection config)
    {
        services.Configure<PollingStationParserConfig>(config.GetSection(PollingStationParserConfig.Key));

        services.AddSingleton<IPollingStationParser, PollingStationParser>();
        return services;
    }
}
