using Microsoft.Extensions.DependencyInjection;

namespace Feature.IssueReports.Notes;

public static class IssueReportsNotesInstaller
{
    public static IServiceCollection AddIssueReportsNotesFeature(this IServiceCollection services)
    {
        return services;
    }
}
