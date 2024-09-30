using Microsoft.Extensions.DependencyInjection;

namespace Feature.IncidentsReports.Attachments;

public static class IncidentReportsAttachmentsFeatureInstaller
{
    public static IServiceCollection AddIncidentReportAttachmentsFeature(this IServiceCollection services)
    {
        return services;
    }
}