namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// Runs the monitoring NGOs' automation rules on a record an observer or citizen just saved, for every NGO that can
/// see it. Call it after the record is saved; it never throws, so a failing rule never fails the submission.
/// </summary>
public interface IRecordAutomation
{
    /// <summary>A new record: severity rules, then the reviewer rules of the NGO's default stage, which it enters.</summary>
    Task RecordCreatedAsync(Guid electionRoundId, ReportRecordType recordType, Guid recordId, CancellationToken ct);

    /// <summary>An edited record (answers changed, moved to another place): severity rules.</summary>
    Task RecordEditedAsync(Guid electionRoundId, ReportRecordType recordType, Guid recordId, CancellationToken ct);
}
