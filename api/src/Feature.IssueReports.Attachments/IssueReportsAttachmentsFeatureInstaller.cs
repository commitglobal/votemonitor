using Microsoft.Extensions.DependencyInjection;

namespace Feature.IssueReports.Attachments;

public static class IssueReportsAttachmentsFeatureInstaller
{
    public static IServiceCollection AddIssueReportsAttachmentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
