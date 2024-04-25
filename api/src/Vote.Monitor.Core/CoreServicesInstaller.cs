using Hangfire;
using Hangfire.PostgreSql;
using Job.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Extensions;
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
        services.AddSingleton(typeof(ICsvReader<>), typeof(CsvReader<>));
        services.AddSingleton(typeof(ICsvWriter<>), typeof(CsvWriter<>));

        services.AddSingleton<ISerializerService, SerializerService>();

        services.AddSingleton<CurrentUtcTimeProvider>();
        services.AddScoped<ITimeProvider>(sp =>
        {
            var currentUtc = sp.GetRequiredService<CurrentUtcTimeProvider>();

            return new FreezeTimeProvider(currentUtc);
        });

        var enableHangfire = configuration.GetValue<bool>("EnableHangfire");
        if (enableHangfire)
        {
            services.AddHangfire(config =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(c =>
                        c.UseNpgsqlConnection(configuration.GetNpgsqlConnectionString("HangfireConnectionConfig")), new PostgreSqlStorageOptions() { PrepareSchemaIfNecessary = false });

                config.UseColouredConsoleLogProvider();
            });

            services.AddTransient<IJobService, HangfireJobService>();
        }
        else
        {
            services.AddTransient<IJobService, NoopJobService>();
        }

        services.AddTransient<IEmailTemplateFactory, EmailTemplateFactory>();

        return services;
    }

    internal class NoopJobService : IJobService
    {
        public void SendEmail(string to, string subject, string body)
        {
        }

        public string ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
        {
            return $"noop-job-{Guid.NewGuid()}";
        }
    }

}
