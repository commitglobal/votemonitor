using NSwag;
using Vote.Monitor.Api.Feature.FormTemplate;
using Vote.Monitor.Api.Feature.Ngo;
using Vote.Monitor.Api.Feature.NgoAdmin;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddApplicationDomain(builder.Configuration.GetSection(DomainInstaller.SectionKey));
builder.Services.AddAuthFeature(builder.Configuration.GetSection(AuthFeatureInstaller.SectionKey));
builder.Services.AddPollingStationFeature(builder.Configuration.GetSection(PollingStationFeatureInstaller.SectionKey));
builder.Services.AddCountryFeature();
builder.Services.AddLanguageFeature();
builder.Services.AddNgoFeature();
builder.Services.AddNgoAdminFeature();
builder.Services.AddObserverFeature(builder.Configuration.GetSection(ObserverFeatureInstaller.SectionKey));
builder.Services.AddElectionRoundFeature();
builder.Services.AddMonitoringFeature();
builder.Services.AddUserPreferencesFeature();
builder.Services.AddFormTemplateFeature();
builder.Services.AddAuthorization();


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
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<FormType, string>());
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<FormTemplateStatus, string>());
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

app.Run();

namespace Vote.Monitor.Api
{
    public partial class Program { };
}
