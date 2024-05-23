using Microsoft.Extensions.DependencyInjection;

namespace Feature.DataExport;

public static class DataExportInstaller
{
    public static IServiceCollection AddDataExportFeature(this IServiceCollection services)
    {
        return services;
    }
}
