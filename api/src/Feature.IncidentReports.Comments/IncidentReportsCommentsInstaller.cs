using Microsoft.Extensions.DependencyInjection;

namespace Feature.IncidentReports.Comments;

public static class IncidentReportsCommentsInstaller
{
    public static IServiceCollection AddIncidentReportsCommentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
