using Feature.Locations.Options;
using Feature.Locations.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Locations;
public static class LocationsFeatureInstaller
{
    public const string SectionKey = "LocationsFeatureConfig";

    public static IServiceCollection AddLocationsFeature(this IServiceCollection services, IConfigurationSection config)
    {
        services.Configure<LocationParserConfig>(config.GetSection(LocationParserConfig.Key));

        services.AddSingleton<ILocationsParser, LocationParser>();
        return services;
    }
}
