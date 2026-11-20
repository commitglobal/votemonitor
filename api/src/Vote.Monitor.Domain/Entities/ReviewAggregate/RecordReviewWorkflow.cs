using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>A record's review stage and reviewers in one NGO, as a record's own GET returns them.</summary>
public sealed record RecordReviewWorkflow(
    RecordReviewStage Stage,
    DateTime? StageChangedAt,
    DateTime? ReviewedAt,
    RecordReviewer[] Reviewers);

public record RecordReviewStage
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required ReviewStatus Status { get; init; }
    public required double Sequence { get; init; }
    public required bool IsDefault { get; init; }
    public Guid? ParentStageId { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }

    public static RecordReviewStage FromEntity(ReviewStage stage) => new()
    {
        Id = stage.Id,
        Name = stage.Name,
        Description = stage.Description,
        Status = stage.Status,
        Sequence = stage.Sequence,
        IsDefault = stage.IsDefault,
        ParentStageId = stage.ParentStageId,
        Color = stage.Color,
        Icon = stage.Icon
    };
}

public record RecordReviewer
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required UserStatus Status { get; init; }
}

public static class RecordReviewWorkflowQueries
{
    /// <summary>
    /// The record as the caller's NGO sees it (for its review stage, severity and labels); null when the NGO doesn't
    /// monitor the round (platform admins have no NGO).
    /// </summary>
    public static async Task<ReviewTarget?> GetReviewTargetAsync(this VoteMonitorContext context,
        Guid electionRoundId,
        Guid ngoId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct)
    {
        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(electionRoundId, ngoId, ct);
        return monitoringNgoId is null ? null : new ReviewTarget(electionRoundId, monitoringNgoId.Value, recordType, recordId);
    }

    /// <summary>
    /// The record's review stage and reviewers in the NGO. A record nobody moved is in the NGO's default stage; null
    /// when the NGO has no stages.
    /// </summary>
    public static async Task<RecordReviewWorkflow?> GetReviewWorkflowAsync(this VoteMonitorContext context,
        ReviewTarget target,
        CancellationToken ct)
    {
        var workflow = await context.ReviewWorkflows
            .AsNoTracking()
            .Include(x => x.Stage)
            .FirstOrDefaultAsync(x => x.MonitoringNgoId == target.MonitoringNgoId
                                      && x.RecordType == target.RecordType
                                      && x.RecordId == target.RecordId, ct);
        var stage = workflow?.Stage ?? await context.ReviewStages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MonitoringNgoId == target.MonitoringNgoId && x.IsDefault, ct);
        if (stage is null)
        {
            return null;
        }

        var reviewers = await context.Reviewers
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == target.MonitoringNgoId
                        && x.RecordType == target.RecordType
                        && x.RecordId == target.RecordId)
            .Join(context.Users, reviewer => reviewer.UserId, user => user.Id, (_, user) => user)
            .OrderBy(x => x.DisplayName)
            .Select(x => new RecordReviewer { Id = x.Id, DisplayName = x.DisplayName, Status = x.Status })
            .ToArrayAsync(ct);

        return new RecordReviewWorkflow(RecordReviewStage.FromEntity(stage), workflow?.StageChangedAt,
            workflow?.ReviewedAt, reviewers);
    }
}
