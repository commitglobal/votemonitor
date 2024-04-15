using Feature.MonitoringObservers.Parser;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Parser;

namespace Feature.MonitoringObservers;

public static class MonitoringObserverFeatureInstaller
{
    public static IServiceCollection AddMonitoringObserversFeature(this IServiceCollection services)
    {
        services.AddSingleton<Validator<MonitoringObserverImportModel>, MonitoringObserverImportModelValidator>();
        services.AddSingleton<ICsvParser<MonitoringObserverImportModel>, CsvParser<MonitoringObserverImportModel, MonitoringObserverImportModelMapper>>();

        return services;
    }
}
