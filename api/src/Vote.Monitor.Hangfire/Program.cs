using Serilog;
using Vote.Monitor.Core.Services.FileStorage;
using Vote.Monitor.Core.Services.Mailing;
using Vote.Monitor.Domain;
using Hangfire;
using Hangfire.PostgreSql;
using Job.Contracts.RecurringJobs;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Hangfire.RecurringJobs;
using Sentry.Protocol;
using Hangfire.Dashboard;
using Vote.Monitor.Hangfire;
using Job.Contracts.Jobs;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Hangfire.Jobs;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions();

builder.Services.AddLogging(logging =>
{
    Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

    var loggerConfiguration = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentUserName();

    var logger = Log.Logger = loggerConfiguration.CreateLogger();

    logging.AddSerilog(logger);
});


builder.Services.AddSingleton<ISerializerService, SerializerService>();

builder.Services.AddSingleton<CurrentUtcTimeProvider>();
builder.Services.AddScoped<ITimeProvider>(sp =>
{
    var currentUtc = sp.GetRequiredService<CurrentUtcTimeProvider>();

    return new FreezeTimeProvider(currentUtc);
});

builder.Services.AddSingleton<ICurrentUserIdProvider, MockCurrentUserIdProvider>();

// Register jobs
builder.Services.AddScoped<IRecurringJobManager, RecurringJobManager>();
builder.Services.AddScoped<IAuditLogCleanerJob, AuditLogCleanerJob>();
builder.Services.AddScoped<ISendEmailJob, SendEmailJob>();

builder.Services.AddApplicationDomain(builder.Configuration.GetRequiredSection(DomainInstaller.SectionKey));
builder.Services.AddMailing(builder.Configuration.GetRequiredSection(MailingInstaller.SectionKey));
var builtProvider = builder.Services.BuildServiceProvider();

builder.Services.AddHangfire(config =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c =>
        {
            c.UseNpgsqlConnection(builder.Configuration.GetNpgsqlConnectionString("HangfireConnectionConfig"));
        });

    config.UseActivator(new ContainerJobActivator(builtProvider));

    config.UseColouredConsoleLogProvider();
});
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard();
app.ScheduleRecurringJobs();


app.Run();
