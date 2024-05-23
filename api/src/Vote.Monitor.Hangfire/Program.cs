using Serilog;
using Vote.Monitor.Core.Services.Mailing;
using Vote.Monitor.Domain;
using Hangfire;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Hangfire.RecurringJobs;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Hangfire.Jobs;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Hangfire.Extensions;
using Vote.Monitor.Hangfire.Jobs.ExportData;
using Vote.Monitor.Hangfire.Services;
using Dapper;
using Vote.Monitor.Core.Converters;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Core.Services.FileStorage;
using Vote.Monitor.Core.Services.Hangfire;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;
using Ardalis.SmartEnum.Dapper;

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

#region Register type handleers for Dapper
SqlMapper.AddTypeHandler(typeof(UserStatus), new SmartEnumByValueTypeHandler<UserStatus, string>());
SqlMapper.AddTypeHandler(typeof(UserRole), new SmartEnumByValueTypeHandler<UserRole, string>());
SqlMapper.AddTypeHandler(typeof(NgoStatus), new SmartEnumByValueTypeHandler<NgoStatus, string>());
SqlMapper.AddTypeHandler(typeof(ElectionRoundStatus), new SmartEnumByValueTypeHandler<ElectionRoundStatus, string>());
SqlMapper.AddTypeHandler(typeof(SortOrder), new SmartEnumByValueTypeHandler<SortOrder, string>());
SqlMapper.AddTypeHandler(typeof(FormTemplateType), new SmartEnumByValueTypeHandler<FormTemplateType, string>());
SqlMapper.AddTypeHandler(typeof(FormTemplateStatus), new SmartEnumByValueTypeHandler<FormTemplateStatus, string>());
SqlMapper.AddTypeHandler(typeof(MonitoringNgoStatus), new SmartEnumByValueTypeHandler<MonitoringNgoStatus, string>());
SqlMapper.AddTypeHandler(typeof(MonitoringObserverStatus), new SmartEnumByValueTypeHandler<MonitoringObserverStatus, string>());
SqlMapper.AddTypeHandler(typeof(RatingScale), new SmartEnumByValueTypeHandler<RatingScale, string>());
SqlMapper.AddTypeHandler(typeof(FormType), new SmartEnumByValueTypeHandler<FormType, string>());
SqlMapper.AddTypeHandler(typeof(ExportedDataStatus), new SmartEnumByValueTypeHandler<ExportedDataStatus, string>());
SqlMapper.AddTypeHandler(typeof(QuickReportLocationType), new SmartEnumByValueTypeHandler<QuickReportLocationType, string>());
SqlMapper.AddTypeHandler(typeof(DisplayLogicCondition), new SmartEnumByValueTypeHandler<DisplayLogicCondition, string>());
SqlMapper.AddTypeHandler(typeof(SubmissionFollowUpStatus), new SmartEnumByValueTypeHandler<SubmissionFollowUpStatus, string>());
SqlMapper.AddTypeHandler(typeof(QuickReportFollowUpStatus), new SmartEnumByValueTypeHandler<QuickReportFollowUpStatus, string>());

#endregion

builder.Services.AddSingleton<ISerializerService, SerializerService>();

builder.Services.AddSingleton<CurrentUtcTimeProvider>();
builder.Services.AddScoped<ITimeProvider>(sp =>
{
    var currentUtc = sp.GetRequiredService<CurrentUtcTimeProvider>();

    return new FreezeTimeProvider(currentUtc);
});

builder.Services.AddSingleton<ICurrentUserProvider, MockCurrentUserProvider>();

builder.Services.AddApplicationDomain(builder.Configuration.GetRequiredSection(DomainInstaller.SectionKey));
builder.Services.AddMailing(builder.Configuration.GetRequiredSection(MailingInstaller.SectionKey));
builder.Services.AddFileStorage(builder.Configuration.GetRequiredSection(FileStorageInstaller.SectionKey));

#region register jobs
builder.Services.AddScoped<IRecurringJobManager, RecurringJobManager>();
builder.Services.AddScoped<AuditLogCleanerJob>();
builder.Services.AddScoped<ExportedDataCleanerJob>();
builder.Services.AddScoped<ImportValidationErrorsCleanerJob>();

builder.Services.AddScoped<ISendEmailJob, SendEmailJob>();
builder.Services.AddScoped<IExportFormSubmissionsJob, ExportFormSubmissionsJob>();
#endregion

builder.Services.AddHangfire((sp,config) =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c =>
        {
            c.UseNpgsqlConnection(builder.Configuration.GetNpgsqlConnectionString("Core:HangfireConnectionConfig"));
        });

    config.UseActivator(new ContainerJobActivator(sp));
    config.UseFilter(new AutomaticRetryAttribute { Attempts = 5 });

    config.UseSerilogLogProvider();
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
