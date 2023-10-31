global using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using Vote.Monitor.Feature.PollingStation;
using Vote.Monitor.Domain;

var builder = WebApplication.CreateBuilder();

builder.Services.AddOptions();
builder.Services.DomainSetupStartup(builder.Configuration);
builder.Services.AddPollingStationFeatures(builder.Configuration);
builder.Services.AddFastEndpoints();
builder.Services.AddAuthorization();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Vote Monitor API";
        s.Version = "v2";
    };
    //o.AutoTagPathSegmentIndex = 2;

});

var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

builder.Logging.AddSerilog(logger);

var app = builder.Build();

app.UseAuthorization();
app.UseDefaultExceptionHandler()
   .UseFastEndpoints();
app.UseSwaggerGen();


app.Run();
public partial class Program { };
