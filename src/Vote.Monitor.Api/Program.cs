using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using Vote.Monitor.Auth;
using Vote.Monitor.Core;
using Vote.Monitor.Country;
using Vote.Monitor.CSO;
using Vote.Monitor.CSOAdmin;
using Vote.Monitor.Domain;
using Vote.Monitor.Feature.PollingStation;
using Vote.Monitor.Observer;

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

builder.Services.AddCoreServices();
builder.Services.AddApplicationDomain(builder.Configuration.GetSection(DomainInstaller.SectionKey));
builder.Services.AddAuthFeature(builder.Configuration.GetSection(AuthFeatureInstaller.SectionKey));
builder.Services.AddPollingStationFeature();
builder.Services.AddCountryFeature();
builder.Services.AddCSOFeature();
builder.Services.AddCSOAdminFeature();
builder.Services.AddObserverFeature();
builder.Services.AddFastEndpoints();
builder.Services.AddAuthorization();
builder.Services.SwaggerDocument(o =>
{
    o.AutoTagPathSegmentIndex = 2;
    o.DocumentSettings = s =>
    {
        s.Title = "Vote Monitor API";
        s.Version = "v2";
    };
    o.AutoTagPathSegmentIndex = 2;
});


var app = builder.Build();
await app.Services.InitializeDatabasesAsync();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(x => x.Errors.ResponseBuilder = ProblemDetails.ResponseBuilder);
app.UseSwaggerGen();

app.Run();

namespace Vote.Monitor.Api
{
    public partial class Program { };
}
