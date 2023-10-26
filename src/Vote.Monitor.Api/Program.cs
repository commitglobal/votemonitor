using FastEndpoints;
using FastEndpoints.Swagger;
using Vote.Monitor.Auth;
using Vote.Monitor.CSO;
using Vote.Monitor.Domain;
using Vote.Monitor.Feature.Example;

var builder = WebApplication.CreateBuilder();
builder.Services.AddOptions();
builder.Services.AddApplicationDomain(builder.Configuration.GetSection(DomainInstaller.SectionKey));
builder.Services.AddAuthFeature(builder.Configuration.GetSection(AuthFeatureInstaller.SectionKey));
builder.Services.AddCSOFeature();
builder.Services.AddExampleFeatures(builder.Configuration.GetSection(ExampleFeaturesInstaller.SectionKey));
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.AutoTagPathSegmentIndex = 2;
    o.DocumentSettings = s =>
    {
        s.Title = "Vote Monitor API";
        s.Version = "v2";
    };
});


var app = builder.Build();
await app.Services.InitializeDatabasesAsync();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();

namespace Vote.Monitor.Api
{
    public partial class Program { };
}
