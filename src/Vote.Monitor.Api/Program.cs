using Ardalis.SmartEnum.SystemTextJson;
using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using Vote.Monitor.Api.Feature.Auth;
using Vote.Monitor.Api.Feature.Country;
using Vote.Monitor.Api.Feature.CSO;
using Vote.Monitor.Api.Feature.CSOAdmin;
using Vote.Monitor.Api.Feature.Observer;
using Vote.Monitor.Api.Feature.PollingStation;
using Vote.Monitor.Api.Swagger;
using Vote.Monitor.Core;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.CSOAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

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
        s.SchemaProcessors.Add(new SmartEnumSchemaProcessor());
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

builder.Services.AddCoreServices();
builder.Services.AddApplicationDomain(builder.Configuration.GetSection(DomainInstaller.SectionKey));
builder.Services.AddAuthFeature(builder.Configuration.GetSection(AuthFeatureInstaller.SectionKey));
builder.Services.AddPollingStationFeature(builder.Configuration.GetSection(PollingStationFeatureInstaller.SectionKey));
builder.Services.AddCountryFeature();
builder.Services.AddCSOFeature();
builder.Services.AddCSOAdminFeature();
builder.Services.AddObserverFeature();
builder.Services.AddAuthorization();


var app = builder.Build();
await app.Services.InitializeDatabasesAsync();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(x =>
{
    x.Errors.ResponseBuilder = ProblemDetails.ResponseBuilder;

    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<UserStatus, string>());
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<UserRole, string>());
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<CSOStatus, string>());
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<ElectionRoundStatus, string>());
});

app.UseSwaggerGen(uiConfig: cfg =>
{
    cfg.DocExpansion = "list";
});

app.Run();

namespace Vote.Monitor.Api
{
    public partial class Program { };
}
