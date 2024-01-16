using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Csv;
using Vote.Monitor.Core.Services.Security;
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

        services.AddSingleton<CurrentUtc>();
        services.AddScoped<ITimeService>(sp =>
        {
            var currentUtc = sp.GetRequiredService<CurrentUtc>();

            return new TimeFreeze(currentUtc);
        });

        services
        .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

        return services;
    }
}
