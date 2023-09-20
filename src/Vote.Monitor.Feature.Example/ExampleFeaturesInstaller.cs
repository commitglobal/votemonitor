using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Feature.Example.Options;
using Vote.Monitor.Feature.Example.Services;

namespace Vote.Monitor.Feature.Example;

public static class ExampleFeaturesInstaller
{
    public const string SectionKey = "ExampleFeatures";
    public static IServiceCollection AddExampleFeatures(this IServiceCollection services, IConfigurationSection config)
    {
        services.Configure<UsefulOptions>(config.GetSection(UsefulOptions.Key));
        services.AddSingleton<ISomethingSomethingService, SomethingSomethingService>();
        return services;
    }
}
