using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Csv;

namespace Vote.Monitor.Core;

public static class CoreServicesInstaller
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICsvReader<>), typeof(CsvReader<>));
        services.AddSingleton(typeof(ICsvWriter<>), typeof(CsvWriter<>));

        return services;
    }
}
