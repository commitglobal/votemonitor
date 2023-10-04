global using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Feature.Example;
using Vote.Monitor.Feature.PollingStation;
using Vote.Monitor.Core;
using Vote.Monitor.Core.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddOptions();
builder.Services.AddCoreStartup(builder.Configuration);
//builder.Services.AddDbContext<AppDbContext>(options =>
//{

//    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresVoteApiConnctionString"));
//});
builder.Services.AddExampleFeatures(builder.Configuration.GetSection(ExampleFeaturesInstaller.SectionKey));
builder.Services.AddPollingStationFeatures(builder.Configuration.GetSection(PollingStationFeatureInstaller.SectionKey));
builder.Services.AddFastEndpoints();
builder.Services.AddAuthorization();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Vote Monitor API";
        s.Version = "v2";
    };
});

var app = builder.Build();

app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();
public partial class Program { };
