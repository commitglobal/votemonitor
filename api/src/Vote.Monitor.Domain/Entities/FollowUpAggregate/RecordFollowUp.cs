using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.FollowUpAggregate;

/// <summary>
/// One monitoring NGO's follow-up status for one report. A report without a row for a monitoring NGO has that NGO's
/// default status. Coalition members and the leader each keep their own row, and never see each other's.
/// </summary>
public class RecordFollowUp : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public Guid FollowUpStatusId { get; private set; }
    public FollowUpStatusDefinition FollowUpStatus { get; private set; }
    public DateTime ChangedAt { get; private set; }

    /// <summary>The user who last changed it.</summary>
    public Guid? ChangedBy { get; private set; }

    public static RecordFollowUp Create(Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId,
        FollowUpStatusDefinition status,
        Guid? changedBy,
        DateTime now)
    {
        var followUp = new RecordFollowUp
        {
            Id = Guid.NewGuid(),
            ElectionRoundId = electionRoundId,
            MonitoringNgoId = monitoringNgoId,
            RecordType = recordType,
            RecordId = recordId
        };
        followUp.Change(status, changedBy, now);
        return followUp;
    }

    /// <summary>Moves it to another of the NGO's statuses.</summary>
    public void Change(FollowUpStatusDefinition status, Guid? changedBy, DateTime now)
    {
        if (status.MonitoringNgoId != MonitoringNgoId)
        {
            throw new InvalidOperationException("The follow-up status belongs to another monitoring NGO.");
        }

        FollowUpStatusId = status.Id;
        FollowUpStatus = status;
        ChangedBy = changedBy;
        ChangedAt = now;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private RecordFollowUp()
    {
    }
#pragma warning restore CS8618
}
