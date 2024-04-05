using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Parser;

namespace Vote.Monitor.Api.Feature.Observer;

public static class ObserverFeatureInstaller
{
    public static IServiceCollection AddObserverFeature(this IServiceCollection services)
    {
        services.AddSingleton<Validator<ObserverImportModel>, ObserverImportModelValidator>();
        services.AddSingleton<ICsvParser<ObserverImportModel>, CsvParser<ObserverImportModel, ObserverImportModelMapper>>();
        return services;
    }
}
