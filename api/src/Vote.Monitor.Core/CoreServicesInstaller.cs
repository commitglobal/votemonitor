using Hangfire;
using Hangfire.PostgreSql;
using Job.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Options;
using Vote.Monitor.Core.Services.Csv;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.Hangfire;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Core;

public static class CoreServicesInstaller
{
    public const string SectionKey = "Core";

    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiConfiguration>(configuration.GetSection(ApiConfiguration.Key));

        services.AddSingleton(typeof(ICsvReader<>), typeof(CsvReader<>));
        services.AddSingleton(typeof(ICsvWriter<>), typeof(CsvWriter<>));

        services.AddSingleton<ISerializerService, SerializerService>();

        services.AddSingleton<CurrentUtcTimeProvider>();
        services.AddScoped<ITimeProvider>(sp =>
        {
            var currentUtc = sp.GetRequiredService<CurrentUtcTimeProvider>();

            return new FreezeTimeProvider(currentUtc);
        });

        services.AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(configuration.GetNpgsqlConnectionString("HangfireConnectionConfig")));

            config.UseColouredConsoleLogProvider();
        });

        services.AddTransient<IJobService, HangfireJobService>();
        services.AddTransient<IEmailTemplateFactory, EmailFactory>();

        return services;
    }
}
