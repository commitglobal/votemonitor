using Feature.MonitoringObservers.Parser;
using Feature.MonitoringObservers.Services;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Parser;

namespace Feature.MonitoringObservers;

public static class MonitoringObserverFeatureInstaller
{
    public static IServiceCollection AddMonitoringObserversFeature(this IServiceCollection services)
    {
        services.AddSingleton<Validator<MonitoringObserverImportModel>, MonitoringObserverImportModelValidator>();
        services.AddSingleton<ICsvParser<MonitoringObserverImportModel>, CsvParser<MonitoringObserverImportModel, MonitoringObserverImportModelMapper>>();
        services.AddScoped<IObserverImportService, ObserverImportService>();

        return services;
    }
}
