using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordLabelAggregate;

/// <summary>
/// One label one monitoring NGO put on one report: free-form text, trimmed, 1 to <see cref="MaxLength"/> characters. A
/// report's labels are a set (one row per label, compared case-insensitively). Coalition members and the leader each keep
/// their own labels and never see each other's. The user who added it is <see cref="AuditableBaseEntity.CreatedBy"/>.
/// </summary>
public class RecordLabel : AuditableBaseEntity, IAggregateRoot
{
    public const int MaxLength = 64;

    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public string Label { get; private set; }

    /// <summary>When the label was put on the report.</summary>
    public DateTime CreatedAt { get; private set; }

    public static RecordLabel Create(ReviewTarget target, string label, Guid createdBy, DateTime now) =>
        new(Guid.NewGuid(), target.ElectionRoundId, target.MonitoringNgoId, target.RecordType, target.RecordId, label,
            createdBy, now);

    private RecordLabel(Guid id,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId,
        string label,
        Guid createdBy,
        DateTime createdAt)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        RecordType = recordType;
        RecordId = recordId;
        Label = label;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private RecordLabel()
    {
    }
#pragma warning restore CS8618
}
