namespace Feature.NgoCoalitions.Services;

public class FormSubmissionsCleanupService(VoteMonitorContext context) : IFormSubmissionsCleanupService
{
    public async Task CleanupFormSubmissionsAsync(Guid electionRoundId, Guid coalitionId, Guid monitoringNgoId)
    {
        var formIds = await context.CoalitionFormAccess.Where(x =>
                x.CoalitionId == coalitionId && x.Coalition.ElectionRoundId == electionRoundId)
            .Select(x => x.FormId)
            .Distinct()
            .ToListAsync();

        await context
            .FormSubmissions
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId)
            .Where(x => formIds.Contains(x.FormId))
            .ExecuteDeleteAsync();

        await context
            .Attachments
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId && formIds.Contains(x.FormId))
            .ExecuteDeleteAsync();

        await context
            .Notes
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId)
            .Where(x => formIds.Contains(x.FormId))
            .ExecuteDeleteAsync();

        await context
            .IncidentReports
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId)
            .Where(x => formIds.Contains(x.FormId))
            .ExecuteDeleteAsync();

        await context
            .IncidentReportAttachments
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.IncidentReport.MonitoringObserver.MonitoringNgoId)
            .Where(x => formIds.Contains(x.FormId))
            .ExecuteDeleteAsync();
    }

    public async Task CleanupFormSubmissionsAsync(Guid electionRoundId, Guid coalitionId, Guid monitoringNgoId,
        Guid formId)
    {
        if (!await context.CoalitionFormAccess.AnyAsync(x =>
                x.Form.ElectionRoundId == electionRoundId
                && x.FormId == formId
                && x.CoalitionId == coalitionId
                && x.Coalition.ElectionRoundId == electionRoundId))
        {
            return;
        }

        await context
            .FormSubmissions
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId)
            .Where(x => x.FormId == formId)
            .Where(x => x.Form.ElectionRoundId == electionRoundId)
            .ExecuteDeleteAsync();

        await context
            .Attachments
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId)
            .Where(x => x.FormId == formId)
            .Where(x => x.Form.ElectionRoundId == electionRoundId)
            .ExecuteDeleteAsync();

        await context
            .Notes
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId)
            .Where(x => x.FormId == formId)
            .Where(x => x.Form.ElectionRoundId == electionRoundId)
            .ExecuteDeleteAsync();

        await context
            .IncidentReports
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.MonitoringObserver.MonitoringNgoId)
            .Where(x => x.Form.ElectionRoundId == electionRoundId)
            .Where(x => x.FormId == formId)
            .ExecuteDeleteAsync();

        await context
            .IncidentReportAttachments
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => monitoringNgoId == x.IncidentReport.MonitoringObserver.MonitoringNgoId)
            .Where(x => x.FormId == formId)
            .Where(x => x.Form.ElectionRoundId == electionRoundId)
            .ExecuteDeleteAsync();
    }
}
