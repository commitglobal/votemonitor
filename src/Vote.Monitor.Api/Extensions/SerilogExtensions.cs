using Serilog.Events;

namespace Vote.Monitor.Api.Extensions;

public static class SerilogExtensions
{
    public static LoggerConfiguration WriteToSentry(this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        var sentryConfig = configuration.GetSection("Sentry");
        var isEnabled = sentryConfig.GetValue<bool?>("Enabled") ?? false;

        if (isEnabled)
        {
            loggerConfiguration.WriteTo.Sentry(s =>
            {
                s.Dsn = sentryConfig.GetValue<string>("Dsn");
                s.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                s.MinimumEventLevel = LogEventLevel.Error;
            });
        }

        return loggerConfiguration;
    }
}
