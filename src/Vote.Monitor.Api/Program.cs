global using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using Vote.Monitor.Feature.PollingStation;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.DataContext;
using Microsoft.EntityFrameworkCore;

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
    o.AutoTagPathSegmentIndex = 2;

});

var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

builder.Logging.AddSerilog(logger);

var app = builder.Build();

app.UseAuthorization();
app.UseDefaultExceptionHandler()
   .UseFastEndpoints();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<AppDbContext>()!;
context!.Database.Migrate();
app.UseSwaggerUi3(s => s.DocExpansion = "list");
app.UseSwaggerGen();


app.Run();
public partial class Program { };
