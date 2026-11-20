using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// A record's whole review state in one NGO: stage, reviewers, severity and labels. Returned by
/// <c>GET …/reviews/{recordType}/{recordId}</c> and inside each record's own GET (<c>review</c>), so a record's page
/// reads it with the record.
/// </summary>
public record RecordReviewState
{
    public required RecordReviewStage Stage { get; init; }
    public DateTime? StageChangedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public required RecordReviewer[] Reviewers { get; init; }

    /// <summary>The NGO's severity of the record; <c>NotAssessed</c> when it never set one.</summary>
    public required SeverityLevel Severity { get; init; }

    public DateTime? SeverityChangedAt { get; init; }

    /// <summary>The NGO's labels on the record, sorted ignoring case.</summary>
    public required string[] Labels { get; init; }
}

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

public static class RecordReviewStateQueries
{
    /// <summary>
    /// The caller NGO's review state of a record it can see (the caller checks that); null when the NGO doesn't monitor
    /// the round (platform admins have no NGO). A record nobody moved is in the NGO's default stage.
    /// </summary>
    public static async Task<RecordReviewState?> GetReviewStateAsync(this VoteMonitorContext context,
        Guid electionRoundId,
        Guid ngoId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct)
    {
        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(electionRoundId, ngoId, ct);
        if (monitoringNgoId is null)
        {
            return null;
        }

        return await context.GetReviewStateAsync(new ReviewTarget(electionRoundId, monitoringNgoId.Value, recordType, recordId), ct);
    }

    public static async Task<RecordReviewState?> GetReviewStateAsync(this VoteMonitorContext context,
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

        var severity = await context.RecordSeverities
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == target.MonitoringNgoId
                        && x.RecordType == target.RecordType
                        && x.RecordId == target.RecordId)
            .Select(x => new { x.Severity, x.ChangedAt })
            .FirstOrDefaultAsync(ct);

        var labels = await context.RecordLabels
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == target.MonitoringNgoId
                        && x.RecordType == target.RecordType
                        && x.RecordId == target.RecordId)
            .Select(x => x.Label)
            .ToListAsync(ct);

        return new RecordReviewState
        {
            Stage = RecordReviewStage.FromEntity(stage),
            StageChangedAt = workflow?.StageChangedAt,
            ReviewedAt = workflow?.ReviewedAt,
            Reviewers = reviewers,
            Severity = severity?.Severity ?? SeverityLevel.NotAssessed,
            SeverityChangedAt = severity?.ChangedAt,
            Labels = labels
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x, StringComparer.Ordinal)
                .ToArray()
        };
    }
}
