namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// Adds reviewers to a matching record when it enters one of <see cref="StageIds"/> (a new record enters the default
/// stage). It only ever adds reviewers, never removes anyone.
/// </summary>
public class ReviewerRule : AutomationRule
{
    /// <summary>The stages that trigger the rule; a rule left without stages (they were deleted) never runs.</summary>
    public Guid[] StageIds { get; private set; }

    /// <summary>The users to add; those who are no longer active admins or staff of the NGO are skipped.</summary>
    public Guid[] ReviewerIds { get; private set; }

    public override AutomationRuleKind Kind => AutomationRuleKind.Reviewer;

    public static ReviewerRule Create(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        Guid[] stageIds,
        Guid[] reviewerIds) =>
        new(electionRoundId, monitoringNgoId, name, recordTypes, conditions, stageIds, reviewerIds);

    public void Update(string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        Guid[] stageIds,
        Guid[] reviewerIds)
    {
        Update(name, recordTypes, conditions);
        StageIds = stageIds;
        ReviewerIds = reviewerIds;
    }

    public void RemoveStage(Guid stageId) => StageIds = StageIds.Where(x => x != stageId).ToArray();

    private ReviewerRule(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        Guid[] stageIds,
        Guid[] reviewerIds) : base(electionRoundId, monitoringNgoId, name, recordTypes, conditions)
    {
        StageIds = stageIds;
        ReviewerIds = reviewerIds;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private ReviewerRule()
    {
    }
#pragma warning restore CS8618
}
