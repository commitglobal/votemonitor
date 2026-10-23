using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

/// <summary>
/// One monitoring NGO's severity for one report. A report without a row for a monitoring NGO has severity
/// <see cref="SeverityLevel.None"/> there. Coalition members and the leader each keep their own row, and never see each
/// other's.
/// </summary>
public class RecordSeverity : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public SeverityLevel Severity { get; private set; }
    public DateTime ChangedAt { get; private set; }

    /// <summary>The user who last changed the severity; null when an automation rule did.</summary>
    public Guid? ChangedBy { get; private set; }

    public static RecordSeverity Create(Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId,
        SeverityLevel severity,
        Guid? changedBy,
        DateTime now) =>
        new(Guid.NewGuid(), electionRoundId, monitoringNgoId, recordType, recordId, severity, changedBy, now);

    /// <summary>Changes the severity; returns false (and changes nothing) when it already has this value.</summary>
    public bool Change(SeverityLevel severity, Guid? changedBy, DateTime now)
    {
        if (Severity == severity)
        {
            return false;
        }

        Severity = severity;
        ChangedBy = changedBy;
        ChangedAt = now;
        return true;
    }

    private RecordSeverity(Guid id,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId,
        SeverityLevel severity,
        Guid? changedBy,
        DateTime changedAt)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        RecordType = recordType;
        RecordId = recordId;
        Severity = severity;
        ChangedBy = changedBy;
        ChangedAt = changedAt;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private RecordSeverity()
    {
    }
#pragma warning restore CS8618
}
