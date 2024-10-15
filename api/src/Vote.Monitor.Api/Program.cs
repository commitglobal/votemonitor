using System.IO.Compression;
using System.Text.Json.Serialization;
using Authorization.Policies;
using Feature.Attachments;
using Feature.Form.Submissions;
using Feature.Forms;
using Feature.FormTemplates;
using Feature.MonitoringObservers;
using Feature.Notes;
using Feature.ObserverGuide;
using Feature.PollingStation.Information;
using Feature.PollingStation.Information.Form;
using Feature.PollingStation.Visit;
using Feature.QuickReports;
using Microsoft.AspNetCore.ResponseCompression;
using NSwag;
using Vote.Monitor.Api.Feature.Ngo;
using Vote.Monitor.Api.Feature.NgoAdmin;
using Feature.Notifications;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Core.Services.FileStorage;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.Api.Extensions;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Module.Notifications;
using Ardalis.SmartEnum.Dapper;
using Dapper;
using Feature.CitizenReports;
using Feature.CitizenReports.Attachments;
using Feature.Citizen.Guides;
using Feature.Citizen.Notifications;
using Feature.CitizenReports.Notes;
using Feature.DataExport;
using Feature.Feedback;
using Feature.ImportErrors;
using Feature.IncidentReports;
using Feature.IncidentReports.Attachments;
using Feature.IncidentReports.Notes;
using Feature.Locations;
using Feature.Monitoring;
using Feature.Statistics;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Microsoft.AspNetCore.Http.Features;
using Vote.Monitor.Core.Converters;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = Int64.MaxValue; //set to max allowed file size of your system
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.AddSentry();

builder.Services.AddFastEndpoints();
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.SwaggerDocument(o =>
{
    o.FlattenSchema = true;
    o.AutoTagPathSegmentIndex = 2;
    o.TagCase = TagCase.LowerCase;

    o.DocumentSettings = s =>
    {
        s.Title = "Vote Monitor API";
        s.Version = "v2";
        s.SchemaSettings.SchemaProcessors.Add(new SmartEnumSchemaProcessor());
        s.SchemaSettings.SchemaProcessors.Add(new GuidSchemaProcessor());
    };
});

builder.Services.AddMemoryCache();
builder.Services.AddOptions();

builder.Services.AddLogging(logging =>
{
    Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

    var loggerConfiguration = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentUserName()
        .WriteToSentry(builder.Configuration)
        .Destructure.ToMaximumDepth(3);

    var logger = Log.Logger = loggerConfiguration.CreateLogger();

    logging.AddSerilog(logger);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddCoreServices(builder.Configuration.GetRequiredSection(CoreServicesInstaller.SectionKey));
builder.Services.AddFileStorage(builder.Configuration.GetRequiredSection(FileStorageInstaller.SectionKey));

builder.Services.AddPushNotifications(builder.Configuration.GetRequiredSection(PushNotificationsInstaller.SectionKey));

builder.Services.AddApplicationDomain(builder.Configuration.GetSection(DomainInstaller.SectionKey),
    builder.Environment.IsProduction());
builder.Services.AddSeeders();
builder.Services.AddIdentity();

builder.Services.AddAuthFeature(builder.Configuration.GetSection(AuthFeatureInstaller.SectionKey));
builder.Services.AddAuthorizationPolicies();
builder.Services.AddPollingStationFeature(builder.Configuration.GetSection(PollingStationFeatureInstaller.SectionKey));
builder.Services.AddCountryFeature();
builder.Services.AddLanguageFeature();
builder.Services.AddNgoFeature();
builder.Services.AddNgoAdminFeature();
builder.Services.AddObserverFeature();
builder.Services.AddElectionRoundFeature();
builder.Services.AddMonitoringFeature();
builder.Services.AddUserPreferencesFeature();
builder.Services.AddFormTemplateFeature();
builder.Services.AddAttachmentsFeature();
builder.Services.AddNotesFeature();
builder.Services.AddPushNotificationsFeature();
builder.Services.AddPollingStationInformationFeature();
builder.Services.AddFormFeature();
builder.Services.AddPollingStationInformationFormFeature();
builder.Services.AddObserverGuideFeature();
builder.Services.AddPollingStationVisitFeature();
builder.Services.AddMonitoringObserversFeature();
builder.Services.AddFormSubmissionsFeature();
builder.Services.AddQuickReportsFeature();
builder.Services.AddImportErrorsFeature();
builder.Services.AddDataExportFeature();
builder.Services.AddStatisticsFeature(builder.Configuration.GetRequiredSection(StatisticsInstaller.SectionKey));
builder.Services.AddFeedbackFeature();

builder.Services.AddCitizenReportsFeature();
builder.Services.AddCitizenReportsNotesFeature();
builder.Services.AddCitizenReportsAttachmentsFeature();
builder.Services.AddCitizenGuidesFeature();
builder.Services.AddLocationsFeature(builder.Configuration.GetSection(LocationsFeatureInstaller.SectionKey));
builder.Services.AddIncidentReportsFeature();
builder.Services.AddIncidentReportsNotesFeature();
builder.Services.AddIncidentReportAttachmentsFeature();
builder.Services.AddCitizenNotificationsFeature();

builder.Services.AddAuthorization();

builder.Services.AddResponseCompression(opts =>
    {
        opts.EnableForHttps = true;
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }).ToArray();
        opts.Providers.Add<BrotliCompressionProvider>();
        opts.Providers.Add<GzipCompressionProvider>();
    })
    .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
    .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);


var app = builder.Build();
await app.Services.InitializeDatabasesAsync();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseSentryMiddleware()
    .UseFastEndpoints(x =>
    {
        x.Errors.UseProblemDetails();
        x.Endpoints.Configurator = ep => { ep.PreProcessor<CurrentUserInjector>(Order.Before); };

        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<UserStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<UserRole, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<NgoStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<ElectionRoundStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<SortOrder, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<FormTemplateType, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<FormTemplateStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<MonitoringNgoStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<MonitoringObserverStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<RatingScale, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<FormType, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<ExportedDataStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<QuickReportLocationType, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<DisplayLogicCondition, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<SubmissionFollowUpStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<QuickReportFollowUpStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<TranslationStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<CitizenReportFollowUpStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<QuestionsAnsweredFilter, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<ObserverGuideType, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<CitizenGuideType, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<ExportedDataType, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<IncidentReportLocationType, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<IncidentReportFollowUpStatus, string>());
        x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<IncidentCategory, string>());

        x.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

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
SqlMapper.AddTypeHandler(typeof(FormStatus), new SmartEnumByValueTypeHandler<FormStatus, string>());
SqlMapper.AddTypeHandler(typeof(ExportedDataStatus), new SmartEnumByValueTypeHandler<ExportedDataStatus, string>());
SqlMapper.AddTypeHandler(typeof(QuickReportLocationType), new SmartEnumByValueTypeHandler<QuickReportLocationType, string>());
SqlMapper.AddTypeHandler(typeof(DisplayLogicCondition), new SmartEnumByValueTypeHandler<DisplayLogicCondition, string>());
SqlMapper.AddTypeHandler(typeof(SubmissionFollowUpStatus), new SmartEnumByValueTypeHandler<SubmissionFollowUpStatus, string>());
SqlMapper.AddTypeHandler(typeof(QuickReportFollowUpStatus), new SmartEnumByValueTypeHandler<QuickReportFollowUpStatus, string>());
SqlMapper.AddTypeHandler(typeof(CitizenReportFollowUpStatus), new SmartEnumByValueTypeHandler<CitizenReportFollowUpStatus, string>());
SqlMapper.AddTypeHandler(typeof(IncidentReportFollowUpStatus), new SmartEnumByValueTypeHandler<IncidentReportFollowUpStatus, string>());
SqlMapper.AddTypeHandler(typeof(IncidentReportLocationType), new SmartEnumByValueTypeHandler<IncidentReportLocationType, string>());
SqlMapper.AddTypeHandler(typeof(IncidentCategory), new SmartEnumByValueTypeHandler<IncidentCategory, string>());


SqlMapper.AddTypeHandler(typeof(TranslatedString), new JsonToObjectConverter<TranslatedString>());
SqlMapper.AddTypeHandler(typeof(LanguagesTranslationStatus), new JsonToObjectConverter<LanguagesTranslationStatus>());

#endregion

app.UseSwaggerGen(
    cfg =>
    {
        cfg.PostProcess = (document, _) =>
        {
            var commitHash = Environment.GetEnvironmentVariable("COMMIT_HASH")?[..7] ?? "Unknown";

            document.Info = new OpenApiInfo
            {
                Version = "v2.0",
                Title = $"Vote Monitor API({commitHash})",
                Description = "An ASP.NET Core Web API for monitoring elections.",
                ExtensionData = new Dictionary<string, object?>
                {
                    ["commit-hash"] = commitHash
                },
                Contact = new OpenApiContact
                {
                    Name = "CommitGlobal",
                    Url = "https://www.commitglobal.org/en/contact-us"
                },
                License = new OpenApiLicense
                {
                    Name = "MPL-2.0 license",
                    Url = "https://github.com/commitglobal/votemonitor/blob/main/LICENSE"
                }
            };
        };
    },
    uiConfig: cfg => { cfg.DocExpansion = "list"; });
app.UseResponseCompression();
app.MapHealthChecks("/health");

app.Run();

namespace Vote.Monitor.Api
{
    public partial class Program;
}