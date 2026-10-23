using Feature.Severity.Rules;
using Feature.Severity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Severity;

public static class SeverityFeatureInstaller
{
    public static IServiceCollection AddSeverityFeature(this IServiceCollection services)
    {
        services.AddScoped<ISeverityService, SeverityService>();
        services.AddScoped<ISeverityRuleRunner, SeverityRuleRunner>();
        services.AddScoped<IRecordAutomation, RecordAutomation>();
        return services;
    }
}
