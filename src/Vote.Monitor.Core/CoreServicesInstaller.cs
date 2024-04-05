using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Csv;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Core;

public static class CoreServicesInstaller
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICsvReader<>), typeof(CsvReader<>));
        services.AddSingleton(typeof(ICsvWriter<>), typeof(CsvWriter<>));

        services.AddSingleton<ISerializerService, SerializerService>();

        services.AddSingleton<CurrentUtcTimeProvider>();
        services.AddScoped<ITimeProvider>(sp =>
        {
            var currentUtc = sp.GetRequiredService<CurrentUtcTimeProvider>();

            return new FreezeTimeProvider(currentUtc);
        });

        return services;
    }
}
