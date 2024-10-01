using Microsoft.Extensions.DependencyInjection;

namespace Feature.IncidentsReports.Notes;

public static class IncidentReportsNotesInstaller
{
    public static IServiceCollection AddIncidentReportsNotesFeature(this IServiceCollection services)
    {
        return services;
    }
}
