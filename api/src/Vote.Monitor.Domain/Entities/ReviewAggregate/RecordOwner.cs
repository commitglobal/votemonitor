namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>The monitoring NGO whose observer sent a record, and that observer's email.</summary>
public sealed record RecordOwner(Guid NgoId, string NgoName, string ObserverEmail);

public static class RecordOwnerQueries
{
    /// <summary>
    /// Who a form submission (or PSI submission), quick report or incident report belongs to; null when the round has
    /// no such record. Platform admins read a record as its owning NGO does, before what they mustn't see is removed.
    /// </summary>
    public static async Task<RecordOwner?> GetRecordOwnerAsync(this VoteMonitorContext context,
        Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct)
    {
        Guid? monitoringObserverId = null;
        if (recordType == ReportRecordType.FormSubmission)
        {
            monitoringObserverId = await context.FormSubmissions
                                       .Where(x => x.ElectionRoundId == electionRoundId && x.Id == recordId)
                                       .Select(x => (Guid?)x.MonitoringObserverId)
                                       .FirstOrDefaultAsync(ct)
                                   ?? await context.PollingStationInformation
                                       .Where(x => x.ElectionRoundId == electionRoundId && x.Id == recordId)
                                       .Select(x => (Guid?)x.MonitoringObserverId)
                                       .FirstOrDefaultAsync(ct);
        }
        else if (recordType == ReportRecordType.QuickReport)
        {
            monitoringObserverId = await context.QuickReports
                .Where(x => x.ElectionRoundId == electionRoundId && x.Id == recordId)
                .Select(x => (Guid?)x.MonitoringObserverId)
                .FirstOrDefaultAsync(ct);
        }
        else if (recordType == ReportRecordType.IncidentReport)
        {
            monitoringObserverId = await context.IncidentReports
                .Where(x => x.ElectionRoundId == electionRoundId && x.Id == recordId)
                .Select(x => (Guid?)x.MonitoringObserverId)
                .FirstOrDefaultAsync(ct);
        }

        if (monitoringObserverId is null)
        {
            return null;
        }

        return await context.MonitoringObservers
            .Where(x => x.Id == monitoringObserverId)
            .Select(x => new RecordOwner(x.MonitoringNgo.NgoId, x.MonitoringNgo.Ngo.Name, x.Observer.ApplicationUser.Email!))
            .FirstOrDefaultAsync(ct);
    }
}
