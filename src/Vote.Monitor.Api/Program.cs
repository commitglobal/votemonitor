using System.IO.Compression;
using Authorization.Policies;
using Feature.ObserverGuide;
using Feature.PollingStation.Information.Form;
using Microsoft.AspNetCore.ResponseCompression;
using NSwag;
using Vote.Monitor.Api.Feature.Answers.Attachments;
using Vote.Monitor.Api.Feature.Answers.Notes;
using Vote.Monitor.Api.Feature.Emergencies;
using Vote.Monitor.Api.Feature.Emergencies.Attachments;
using Vote.Monitor.Api.Feature.Form;
using Vote.Monitor.Api.Feature.FormTemplate;
using Vote.Monitor.Api.Feature.Ngo;
using Vote.Monitor.Api.Feature.NgoAdmin;
using Vote.Monitor.Api.Feature.Notifications;
using Vote.Monitor.Api.Feature.PollingStation.Attachments;
using Vote.Monitor.Api.Feature.PollingStation.Information;
using Vote.Monitor.Api.Feature.PollingStation.Notes;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Core.Services.FileStorage;
using Vote.Monitor.Core.Services.PushNotification;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSentry();

builder.Services.AddFastEndpoints();
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
            .WriteToSentry(builder.Configuration);

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

builder.Services.AddCoreServices();
builder.Services.AddFileStorage(builder.Configuration.GetRequiredSection(FileStorageInstaller.SectionKey));
builder.Services.AddPushNotifications(builder.Configuration.GetRequiredSection(PushNotificationsInstaller.SectionKey));

builder.Services.AddApplicationDomain(builder.Configuration.GetSection(DomainInstaller.SectionKey));
builder.Services.AddAuthorizationPolicies();
builder.Services.AddAuthFeature(builder.Configuration.GetSection(AuthFeatureInstaller.SectionKey));
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
builder.Services.AddPollingStationAttachmentsFeature();
builder.Services.AddPollingStationNotesFeature();
builder.Services.AddPushNotificationsFeature();
builder.Services.AddAnswerNotesFeature();
builder.Services.AddAnswersAttachmentsFeature();
builder.Services.AddEmergenciesFeature();
builder.Services.AddEmergencyAttachmentsFeature();
builder.Services.AddPollingStationInformationFeature();
builder.Services.AddFormFeature();
builder.Services.AddPollingStationInformationFormFeature();
builder.Services.AddObserverGuideFeature();

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

app.UseFastEndpoints(x =>
{
    x.Errors.UseProblemDetails();
    x.Endpoints.Configurator = ep =>
    {
        ep.PreProcessor<CurrentUserInjector>(Order.Before);
    };

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
});

app.UseSwaggerGen(
cfg =>
{
    cfg.PostProcess = (document, _) =>
    {
        var commitHash = Environment.GetEnvironmentVariable("COMMIT_HASH") ?? "Unknown";

        document.Info = new OpenApiInfo
        {
            Version = "v2.0",
            Title = $"Vote Monitor API({commitHash})",
            Description = $"An ASP.NET Core Web API for monitoring elections.",
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
uiConfig: cfg =>
{
    cfg.DocExpansion = "list";
});
app.UseResponseCompression();
app.UseSentryMiddleware();

app.Run();

namespace Vote.Monitor.Api
{
    public partial class Program;
}
