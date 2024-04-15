using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Api.Feature.PollingStation.Options;
using Vote.Monitor.Api.Feature.PollingStation.Services;

namespace Vote.Monitor.Api.Feature.PollingStation;
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
