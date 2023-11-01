using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Csv;

namespace Vote.Monitor.Core;

public static class CoreServicesInstaller
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICsvReader<>), typeof(CsvReader<>));
        services.AddScoped(typeof(ICsvWriter<>), typeof(CsvWriter<>));

        return services;
    }
}
