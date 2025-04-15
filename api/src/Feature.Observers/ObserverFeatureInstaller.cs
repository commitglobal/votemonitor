using Dapper;
using Feature.Observers.Parser;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Converters;
using Vote.Monitor.Core.Services.Parser;

namespace Feature.Observers;

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
