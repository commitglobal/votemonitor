using Microsoft.Extensions.DependencyInjection;

namespace Feature.IncidentReports.Attachments;

public static class IncidentReportsAttachmentsFeatureInstaller
{
    public static IServiceCollection AddIncidentReportAttachmentsFeature(this IServiceCollection services)
    {
        return services;
    }
}