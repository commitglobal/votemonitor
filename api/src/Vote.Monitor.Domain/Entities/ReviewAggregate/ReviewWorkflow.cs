namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// One monitoring NGO's stage for one report. A report without a row for a monitoring NGO is in that NGO's default
/// stage. Coalition members and the leader each keep their own row, and never see each other's.
/// </summary>
public class ReviewWorkflow : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public Guid StageId { get; private set; }
    public ReviewStage Stage { get; private set; }
    public DateTime StageChangedAt { get; private set; }

    /// <summary>When the report entered a stage of the Reviewed status; cleared when it leaves that status.</summary>
    public DateTime? ReviewedAt { get; private set; }

    public static ReviewWorkflow Create(Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId,
        ReviewStage stage,
        DateTime now)
    {
        var workflow = new ReviewWorkflow(Guid.NewGuid(), electionRoundId, monitoringNgoId, recordType, recordId);
        workflow.SetStage(stage, now);
        return workflow;
    }

    public void SetStage(ReviewStage stage, DateTime now)
    {
        if (stage.MonitoringNgoId != MonitoringNgoId)
        {
            throw new InvalidOperationException("The stage belongs to another monitoring NGO.");
        }

        StageId = stage.Id;
        Stage = stage;
        StageChangedAt = now;
        ReviewedAt = stage.Status == ReviewStatus.Reviewed ? now : null;
    }

    private ReviewWorkflow(Guid id,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        Guid recordId)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        RecordType = recordType;
        RecordId = recordId;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private ReviewWorkflow()
    {
    }
#pragma warning restore CS8618
}
