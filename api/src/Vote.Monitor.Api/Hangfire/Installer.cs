using Hangfire;
using Hangfire.PostgreSql;
using Job.Contracts;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Hangfire;

public static class Installer
{
    public static void AddHangfireBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var enableHangfire = configuration.GetValue<bool>("Core:EnableHangfire");
        if (enableHangfire)
        {
            services.AddHangfire(config =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(c =>
                            c.UseNpgsqlConnection(configuration.GetNpgsqlConnectionString("Core:HangfireConnectionConfig")),
                        new PostgreSqlStorageOptions { PrepareSchemaIfNecessary = false });

                config.UseSerilogLogProvider();
            });

            services.AddTransient<IJobService, HangfireJobService>();
        }
        else
        {
            services.AddTransient<IJobService, NoopJobService>();
        }
    }
   
}