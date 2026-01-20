using Authorization.Policies;
using FastEndpoints;
using FastEndpoints.Swagger;
using NSwag;
using Serilog;
using Vote.Monitor.Core;
using Vote.Monitor.Domain;
using Vote.Monitor.Ingestor.Core.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.FlattenSchema = true;
    o.AutoTagPathSegmentIndex = 2;
    o.TagCase = TagCase.LowerCase;

    o.DocumentSettings = s =>
    {
        s.Title = "Vote Monitor Ingestor API";
        s.Version = "v2";
    };
});

builder.Services.AddCoreServices(builder.Configuration.GetRequiredSection(CoreServicesInstaller.SectionKey));
builder.Services.AddApplicationDomain(builder.Configuration.GetSection(DomainInstaller.SectionKey),
    builder.Environment.IsProduction());

builder.Services.AddIdentity();
builder.Services.AddScoped<SmsToFormSubmissionDecoder>();

//builder.Services.AddAuthFeature(builder.Configuration.GetSection(AuthFeatureInstaller.SectionKey));
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

await app.Services.InitializeDatabasesAsync();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.UseSwaggerGen(
    cfg =>
    {
        cfg.PostProcess = (document, _) =>
        {
            var commitHash = Environment.GetEnvironmentVariable("COMMIT_HASH")?[..7] ?? "Unknown";

            document.Info = new OpenApiInfo
            {
                Version = "v2.0",
                Title = $"Vote Monitor Ingestor API({commitHash})",
                Description = "An ASP.NET Core Web API for ingesting messages related to monitoring elections.",
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
app.UseHttpsRedirection();

app.Run();
