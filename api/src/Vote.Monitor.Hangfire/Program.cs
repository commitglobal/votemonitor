using Serilog;
using Vote.Monitor.Core.Services.Mailing;
using Vote.Monitor.Domain;
using Hangfire;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Hangfire.RecurringJobs;
using Job.Contracts.Jobs;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Hangfire.Jobs;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Hangfire.Extensions;
using Vote.Monitor.Hangfire.Jobs.ExportData;
using Vote.Monitor.Hangfire.Services;
using Dapper;
using Vote.Monitor.Core.Converters;
using Vote.Monitor.Core.Services.FileStorage;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;

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

// Register type handlers;
SqlMapper.AddTypeHandler(typeof(BaseQuestion[]), new JsonToObjectConverter<BaseQuestion[]>());
SqlMapper.AddTypeHandler(typeof(BaseAnswer[]), new JsonToObjectConverter<BaseAnswer[]>());
SqlMapper.AddTypeHandler(typeof(NoteModel[]), new JsonToObjectConverter<NoteModel[]>());
SqlMapper.AddTypeHandler(typeof(AttachmentModel[]), new JsonToObjectConverter<AttachmentModel[]>());


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
builder.Services.AddScoped<IExportedDataCleanerJob, ExportedDataCleanerJob>();
builder.Services.AddScoped<IImportValidationErrorsCleanerJob, ImportValidationErrorsCleanerJob>();

builder.Services.AddScoped<ISendEmailJob, SendEmailJob>();
builder.Services.AddScoped<IExportFormSubmissionsJob, ExportFormSubmissionsJob>();

builder.Services.AddApplicationDomain(builder.Configuration.GetRequiredSection(DomainInstaller.SectionKey));
builder.Services.AddMailing(builder.Configuration.GetRequiredSection(MailingInstaller.SectionKey));
builder.Services.AddFileStorage(builder.Configuration.GetRequiredSection(FileStorageInstaller.SectionKey));

var builtProvider = builder.Services.BuildServiceProvider();

builder.Services.AddHangfire(config =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c =>
        {
            c.UseNpgsqlConnection(builder.Configuration.GetNpgsqlConnectionString("Core:HangfireConnectionConfig"));
        });

    config.UseActivator(new ContainerJobActivator(builtProvider));

    config.UseColouredConsoleLogProvider();
});
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Vote.Monitor.Hangfire",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter{
            User = app.Configuration["DashboardAuth:Username"],
            Pass = app.Configuration["DashboardAuth:Password"]
        }
    }
});

app.ScheduleRecurringJobs();


app.Run();
