using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Sentry.Extensibility;
using Sentry.OpenTelemetry;

namespace Vote.Monitor.Api.Extensions;

public static class SentryExtensions
{
    const string SentryConfigurationSection = "Sentry";
    public static WebApplicationBuilder AddSentry(this WebApplicationBuilder builder)
    {
        var sentryConfig = builder.Configuration.GetSection(SentryConfigurationSection);
        var isSentryEnabled = sentryConfig.GetValue<bool?>("Enabled") ?? false;
        if (!isSentryEnabled)
        {
            return builder;
        }

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
                    tracerProviderBuilder
                        .AddAspNetCoreInstrumentation() // <-- Adds ASP.NET Core telemetry sources
                        .AddHttpClientInstrumentation() // <-- Adds HttpClient telemetry sources
                        .AddSentry() // <-- Configure OpenTelemetry to send trace information to Sentry
            )   // This block configures OpenTelemetry metrics that we care about... later we'll configure Sentry to capture these
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation() // <-- Requires the OpenTelemetry.Instrumentation.Runtime package
                                                 // Collect some of the built-in ASP.NET Core metrics
                    .AddBuiltInMeters();
            });

        builder.WebHost.UseSentry(options =>
        {
            if (builder.Environment.IsProduction())
            {
                var commitHash = Environment.GetEnvironmentVariable("COMMIT_HASH") ?? "Unknown";
                options.Release = commitHash;
            }

            options.Dsn = sentryConfig.GetValue<string>("Dsn")!;
            options.TracesSampleRate = sentryConfig.GetValue<double?>("TracesSampleRate");

            options.SendDefaultPii = true;
            options.MaxRequestBodySize = RequestSize.Always;
            options.MinimumBreadcrumbLevel = LogLevel.Debug;
            options.MinimumEventLevel = LogLevel.Warning;
            options.AttachStacktrace = true;
            options.DiagnosticLevel = SentryLevel.Error;

            options.UseOpenTelemetry(); // <-- Configure Sentry to use OpenTelemetry trace information
            // This shows experimental support for capturing OpenTelemetry metrics with Sentry
            options.ExperimentalMetrics = new ExperimentalMetricsOptions()
            {
                // Here we're telling Sentry to capture all built-in metrics. This includes all the metrics we configured
                // OpenTelemetry to emit when we called `builder.Services.AddOpenTelemetry()` above:
                // - "OpenTelemetry.Instrumentation.Runtime"
                // - "Microsoft.AspNetCore.Hosting",
                // - "Microsoft.AspNetCore.Server.Kestrel",
                // - "System.Net.Http"
                CaptureSystemDiagnosticsMeters = BuiltInSystemDiagnosticsMeters.All
            };
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<ISentryUserFactory, CustomSentryUserFactory>();

        return builder;
    }

    public static IApplicationBuilder UseSentryMiddleware(this IApplicationBuilder app)
    {
        var sentryConfig = app.ApplicationServices.GetRequiredService<IConfiguration>().GetSection(SentryConfigurationSection);
        var isSentryEnabled = sentryConfig.GetValue<bool?>("Enabled") ?? false;

        if (!isSentryEnabled)
        {
            app.UseDefaultExceptionHandler(logStructuredException: true, useGenericReason: true);
            return app;
        }

        app.UseSentryExceptionHandler();
        app.UseSentryTracing();

        return app;
    }

    class SentryExceptionHandler;

    private static IApplicationBuilder UseSentryExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(
            errApp =>
            {
                errApp.Run(
                    async ctx =>
                    {
                        var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();

                        if (exHandlerFeature is not null)
                        {
                            var logger = ctx.Resolve<ILogger<SentryExceptionHandler>>();
                            var route = exHandlerFeature.Endpoint?.DisplayName?.Split(" => ")[0];
                            var exceptionType = exHandlerFeature.Error.GetType().Name;
                            var reason = exHandlerFeature.Error.Message;

                            logger.LogError(exHandlerFeature.Error, "Unhandled exception at: {@route}{@exceptionType}{@reason}{@exception}", route, exceptionType, reason, exHandlerFeature.Error);

                            SentrySdk.CaptureException(exHandlerFeature.Error);

                            ctx.Response.ContentType = "application/problem+json";
                            await ctx.Response.WriteAsJsonAsync(
                                new InternalErrorResponse
                                {
                                    Status = "Internal Server Error!",
                                    Code = ctx.Response.StatusCode,
                                    Reason = "An unexpected error has occurred.",
                                    Note = "See application log for stack trace."
                                });
                        }
                    });
            });

        return app;
    }

    private static MeterProviderBuilder AddBuiltInMeters(this MeterProviderBuilder meterProviderBuilder) =>
        meterProviderBuilder.AddMeter(
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel",
            "System.Net.Http");

}
