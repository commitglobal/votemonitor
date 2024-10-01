using Microsoft.Extensions.DependencyInjection;

namespace Feature.IncidentReports.Notes;

public static class IncidentReportsNotesInstaller
{
    public static IServiceCollection AddIncidentReportsNotesFeature(this IServiceCollection services)
    {
        return services;
    }
}
