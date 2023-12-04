using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Api.Feature.Observer.Services;
using Vote.Monitor.Api.Feature.PollingStation.Services;

namespace Vote.Monitor.Api.Feature.Observer;

public static class ObserverFeatureInstaller
{
    public const string SectionKey = "ObserverFeatureConfig";
    public static IServiceCollection AddObserverFeature(this IServiceCollection services, IConfigurationSection config)
    {
        Console.WriteLine(CsvFileConfig.Key);
        Console.WriteLine(config.GetSection(CsvFileConfig.Key));
        services.Configure<CsvFileConfig>(config.GetSection(CsvFileConfig.Key));
        services.AddSingleton<Validator<ObserverImportModel>, ObserverImportModelValidator>();
        services.AddSingleton<ICsvParser<ObserverImportModel>, CsvParser<ObserverImportModel, ObserverImportModelMapper>>();
        return services;
    }
}
