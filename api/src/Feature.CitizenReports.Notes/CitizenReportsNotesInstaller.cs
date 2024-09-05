using Microsoft.Extensions.DependencyInjection;

namespace Feature.CitizenReports.Notes;

public static class CitizenReportsNotesInstaller
{
    public static IServiceCollection AddCitizenReportsNotesFeature(this IServiceCollection services)
    {
        return services;
    }
}
