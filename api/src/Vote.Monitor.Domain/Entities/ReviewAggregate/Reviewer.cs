namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// An NGO admin or staff member a monitoring NGO assigned to one of the reports it can see. Assignments are per
/// monitoring NGO: a coalition leader never sees who a member NGO assigned, and the other way round.
/// </summary>
public class Reviewer : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    public DateTime AssignedAt { get; private set; }

    /// <summary>The user who assigned the reviewer; null when an automation rule did.</summary>
    public Guid? AssignedBy { get; private set; }

    public static Reviewer Create(Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId,
        Guid userId,
        DateTime assignedAt,
        Guid? assignedBy) =>
        new(Guid.NewGuid(), electionRoundId, monitoringNgoId, recordType, recordId, userId, assignedAt, assignedBy);

    private Reviewer(Guid id,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId,
        Guid userId,
        DateTime assignedAt,
        Guid? assignedBy)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        RecordType = recordType;
        RecordId = recordId;
        UserId = userId;
        AssignedAt = assignedAt;
        AssignedBy = assignedBy;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private Reviewer()
    {
    }
#pragma warning restore CS8618
}
