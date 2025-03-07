using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Api.Feature.Observer.Parser;
using Vote.Monitor.Core.Converters;
using Vote.Monitor.Core.Services.Parser;

namespace Vote.Monitor.Api.Feature.Observer;

public static class ObserverFeatureInstaller
{
    public static IServiceCollection AddObserverFeature(this IServiceCollection services)
    {
        services.AddSingleton<Validator<ObserverImportModel>, ObserverImportModelValidator>();
        services.AddSingleton<ICsvParser<ObserverImportModel>, CsvParser<ObserverImportModel, ObserverImportModelMapper>>();
        SqlMapper.AddTypeHandler(typeof(ObserverModel.MonitoredElectionsDetails[]), new JsonToObjectConverter<ObserverModel.MonitoredElectionsDetails[]>());

        return services;
    }
}
