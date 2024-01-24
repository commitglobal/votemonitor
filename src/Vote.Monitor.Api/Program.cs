﻿var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddCSOFeature();
builder.Services.AddCSOAdminFeature();
builder.Services.AddObserverFeature(builder.Configuration.GetSection(ObserverFeatureInstaller.SectionKey));
builder.Services.AddElectionRoundFeature();
builder.Services.AddMonitoringFeature();
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
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<CSOStatus, string>());
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<ElectionRoundStatus, string>());
    x.Serializer.Options.Converters.Add(new SmartEnumValueConverter<SortOrder, string>());
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
