using Microsoft.Extensions.DependencyInjection;

namespace Feature.QuickReports;

public static class QuickReportsInstaller
{
    public static IServiceCollection AddQuickReportsFeature(this IServiceCollection services)
    {
        return services;
    }
}
