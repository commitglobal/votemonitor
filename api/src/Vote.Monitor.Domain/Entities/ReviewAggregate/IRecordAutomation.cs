namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// Runs the monitoring NGOs' automation rules on a record an observer or citizen just saved, for every NGO that can
/// see it. Call it after the record is saved; it never throws, so a failing rule never fails the submission.
/// </summary>
public interface IRecordAutomation
{
    /// <summary>A new record: severity rules, then the reviewer rules of the NGO's default stage, which it enters.</summary>
    Task RecordCreatedAsync(Guid electionRoundId, ReportRecordType recordType, Guid recordId, CancellationToken ct);

    /// <summary>A record moved to another place by a platform admin: severity rules.</summary>
    Task RecordEditedAsync(Guid electionRoundId, ReportRecordType recordType, Guid recordId, CancellationToken ct);

    /// <summary>
    /// A record its observer or citizen changed (call it only when the content really changed, not on a re-sync):
    /// severity rules, then the record goes back to each NGO's default stage and gets that stage's reviewer rules.
    /// </summary>
    Task RecordChangedByAuthorAsync(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct);

    /// <summary>
    /// An observer changed a note or attachment of a form submission: <see cref="RecordChangedByAuthorAsync"/> for
    /// the submission it belongs to. One made through the v1 API has no submission (<paramref name="submissionId"/> is
    /// empty): it belongs to the observer's submissions of its form at its polling station.
    /// </summary>
    Task FormSubmissionPartChangedByAuthorAsync(Guid electionRoundId,
        Guid submissionId,
        Guid pollingStationId,
        Guid formId,
        Guid monitoringObserverId,
        CancellationToken ct);
}
