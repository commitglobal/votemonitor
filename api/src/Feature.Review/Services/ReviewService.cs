using Feature.Review.Rules;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Feature.Review.Services;

/// <summary>A report as seen by the caller's monitoring NGO: its review stage there and its reviewers there.</summary>
public sealed class ReviewContext
{
    public required ReviewTarget Target { get; init; }
    public required Guid NgoId { get; init; }
    public required ReviewStage Stage { get; init; }
    public ReviewWorkflowEntity? Workflow { get; init; }
    public required List<Reviewer> Reviewers { get; init; }
}

public enum ReviewOutcome
{
    Done,
    Forbidden,
    InvalidStage,
    InvalidReviewers
}

public sealed record SetReviewersResult(ReviewOutcome Outcome, IReadOnlyList<Guid> InvalidUserIds)
{
    public static readonly SetReviewersResult Done = new(ReviewOutcome.Done, []);
    public static readonly SetReviewersResult Forbidden = new(ReviewOutcome.Forbidden, []);
}

public interface IReviewService
{
    /// <summary>The report as seen by the NGO, or null when the NGO doesn't monitor the round or can't see the report.</summary>
    Task<ReviewContext?> ResolveAsync(Guid electionRoundId,
        Guid ngoId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct);

    Task<bool> IsArchivedAsync(Guid electionRoundId, CancellationToken ct);

    Task<ReviewActor?> GetActorAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// NGO admins change any report's stage; staff only the records they review. Then runs the NGO's reviewer rules of
    /// the new stage.
    /// </summary>
    Task<ReviewOutcome> SetStageAsync(ReviewContext report,
        Guid stageId,
        ReviewActor actor,
        bool actorIsNgoAdmin,
        CancellationToken ct);

    /// <summary>
    /// Sends a report its author changed back to the NGO's default stage, when it is in another stage, and records it
    /// in the history. Then runs the NGO's reviewer rules of the default stage.
    /// </summary>
    Task ReturnToDefaultStageAsync(ReviewContext report, CancellationToken ct);

    /// <summary>
    /// Replaces the report's reviewers. NGO admins assign any active admin or staff member of their NGO; staff can
    /// only add or remove themselves.
    /// </summary>
    Task<SetReviewersResult> SetReviewersAsync(ReviewContext report,
        IReadOnlyCollection<Guid> userIds,
        ReviewActor actor,
        bool actorIsNgoAdmin,
        CancellationToken ct);

    /// <summary>
    /// Gives every report the NGO can see and hasn't moved yet an explicit row in the given (current default)
    /// stage, so switching the default doesn't move them. Runs in the caller's transaction.
    /// </summary>
    Task MaterializeDefaultStageAsync(Guid electionRoundId, Guid ngoId, ReviewStage defaultStage, CancellationToken ct);

    /// <summary>
    /// Adds a change made by another feature (e.g. severity) to the report's history and notifies its reviewers
    /// (other than the user who made it). The caller saves the context.
    /// </summary>
    Task AddActivityAsync(ReviewContext report, ReviewActivity activity, CancellationToken ct);
}

internal class ReviewService(
    VoteMonitorContext context,
    IReviewerRuleRunner reviewerRules,
    ITimeProvider timeProvider,
    ILogger<ReviewService> logger) : IReviewService
{
    public const int MaxReviewers = 10;

    public async Task<ReviewContext?> ResolveAsync(Guid electionRoundId,
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

        var isVisible = await context.Database
            .SqlQueryRaw<int>(
                """SELECT 1 AS "Value" FROM "GetVisibleRecordIds"(@electionRoundId, @ngoId, @recordType) V WHERE V."RecordId" = @recordId""",
                new NpgsqlParameter("electionRoundId", electionRoundId),
                new NpgsqlParameter("ngoId", ngoId),
                new NpgsqlParameter("recordType", recordType.Value),
                new NpgsqlParameter("recordId", recordId))
            .AnyAsync(ct);
        if (!isVisible)
        {
            return null;
        }

        var workflow = await context.ReviewWorkflows
            .Include(x => x.Stage)
            .FirstOrDefaultAsync(x => x.MonitoringNgoId == monitoringNgoId
                                      && x.RecordType == recordType
                                      && x.RecordId == recordId, ct);

        var stage = workflow?.Stage ?? await context.ReviewStages
            .FirstAsync(x => x.MonitoringNgoId == monitoringNgoId && x.IsDefault, ct);

        var reviewers = await context.Reviewers
            .Where(x => x.MonitoringNgoId == monitoringNgoId && x.RecordType == recordType && x.RecordId == recordId)
            .ToListAsync(ct);

        return new ReviewContext
        {
            Target = new ReviewTarget(electionRoundId, monitoringNgoId.Value, recordType, recordId),
            NgoId = ngoId,
            Stage = stage,
            Workflow = workflow,
            Reviewers = reviewers
        };
    }

    public Task<bool> IsArchivedAsync(Guid electionRoundId, CancellationToken ct) =>
        context.ElectionRounds.AnyAsync(x => x.Id == electionRoundId && x.Status == ElectionRoundStatus.Archived, ct);

    public async Task<ReviewActor?> GetActorAsync(Guid userId, CancellationToken ct)
    {
        var name = await context.Users.Where(x => x.Id == userId).Select(x => x.DisplayName).FirstOrDefaultAsync(ct);
        return name is null ? null : new ReviewActor(userId, name);
    }

    public async Task<ReviewOutcome> SetStageAsync(ReviewContext report,
        Guid stageId,
        ReviewActor actor,
        bool actorIsNgoAdmin,
        CancellationToken ct)
    {
        if (!actorIsNgoAdmin && report.Reviewers.All(x => x.UserId != actor.Id))
        {
            return ReviewOutcome.Forbidden;
        }

        var stage = await context.ReviewStages
            .FirstOrDefaultAsync(x => x.Id == stageId && x.MonitoringNgoId == report.Target.MonitoringNgoId, ct);
        if (stage is null)
        {
            return ReviewOutcome.InvalidStage;
        }

        if (stage.Id == report.Stage.Id)
        {
            return ReviewOutcome.Done;
        }

        var now = timeProvider.UtcNow;
        if (report.Workflow is null)
        {
            context.ReviewWorkflows.Add(ReviewWorkflowEntity.Create(report.Target.ElectionRoundId,
                report.Target.MonitoringNgoId,
                report.Target.RecordType,
                report.Target.RecordId,
                stage,
                now));
        }
        else
        {
            report.Workflow.SetStage(stage, now);
        }

        var activity = ReviewActivity.StageChanged(report.Target, actor, report.Stage, stage, now);
        context.ReviewActivities.Add(activity);
        await context.NotifyAsync(activity,
            report.Reviewers.Select(x => x.UserId),
            UserNotificationReason.ReviewingRecordChanged,
            ct);

        await context.SaveChangesAsync(ct);

        try
        {
            await reviewerRules.RunAsync(report, stage, ct);
        }
        catch (Exception e)
        {
            // The stage change is saved; a failing rule must not fail it.
            context.ChangeTracker.Clear();
            logger.LogError(e, "Reviewer rules failed for {RecordType} {RecordId}", report.Target.RecordType,
                report.Target.RecordId);
        }

        return ReviewOutcome.Done;
    }

    public async Task ReturnToDefaultStageAsync(ReviewContext report, CancellationToken ct)
    {
        // No row of its own: the report already follows the default stage.
        if (report.Workflow is null)
        {
            return;
        }

        var defaultStage = await context.ReviewStages
            .FirstAsync(x => x.MonitoringNgoId == report.Target.MonitoringNgoId && x.IsDefault, ct);
        if (defaultStage.Id == report.Stage.Id)
        {
            return;
        }

        var now = timeProvider.UtcNow;
        report.Workflow.SetStage(defaultStage, now);

        var activity = ReviewActivity.ReturnedToDefaultStage(report.Target, report.Stage, defaultStage, now);
        context.ReviewActivities.Add(activity);
        await context.NotifyAsync(activity,
            report.Reviewers.Select(x => x.UserId),
            UserNotificationReason.ReviewingRecordChanged,
            ct);

        await context.SaveChangesAsync(ct);
        await reviewerRules.RunAsync(report, defaultStage, ct);
    }

    public async Task<SetReviewersResult> SetReviewersAsync(ReviewContext report,
        IReadOnlyCollection<Guid> userIds,
        ReviewActor actor,
        bool actorIsNgoAdmin,
        CancellationToken ct)
    {
        var requested = userIds.ToHashSet();
        var current = report.Reviewers.Select(x => x.UserId).ToHashSet();
        var added = requested.Except(current).ToList();
        var removed = current.Except(requested).ToList();

        if (!actorIsNgoAdmin && added.Concat(removed).Any(x => x != actor.Id))
        {
            return SetReviewersResult.Forbidden;
        }

        if (requested.Count > MaxReviewers)
        {
            return new SetReviewersResult(ReviewOutcome.InvalidReviewers, added);
        }

        var assignable = await context.GetAssignableUsersAsync(report.NgoId, added, ct);
        var invalid = added.Where(x => !assignable.ContainsKey(x)).ToList();
        if (invalid.Count > 0)
        {
            return new SetReviewersResult(ReviewOutcome.InvalidReviewers, invalid);
        }

        if (added.Count == 0 && removed.Count == 0)
        {
            return SetReviewersResult.Done;
        }

        var removedNames = await context.Users
            .Where(x => removed.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.DisplayName, ct);
        var remaining = current.Intersect(requested).ToList();
        var now = timeProvider.UtcNow;

        foreach (var userId in added)
        {
            context.Reviewers.Add(Reviewer.Create(report.Target.ElectionRoundId,
                report.Target.MonitoringNgoId,
                report.Target.RecordType,
                report.Target.RecordId,
                userId,
                now,
                actor.Id));

            var activity = ReviewActivity.ReviewerAdded(report.Target, actor, userId, assignable[userId], now);
            context.ReviewActivities.Add(activity);
            await context.NotifyAsync(activity, [userId], UserNotificationReason.AddedAsReviewer, ct);
            await context.NotifyAsync(activity, remaining, UserNotificationReason.ReviewingRecordChanged, ct);
        }

        foreach (var userId in removed)
        {
            context.Reviewers.Remove(report.Reviewers.First(x => x.UserId == userId));

            var activity = ReviewActivity.ReviewerRemoved(report.Target,
                actor,
                userId,
                removedNames.GetValueOrDefault(userId, ""),
                now);
            context.ReviewActivities.Add(activity);
            await context.NotifyAsync(activity, [userId], UserNotificationReason.RemovedAsReviewer, ct);
            await context.NotifyAsync(activity, remaining, UserNotificationReason.ReviewingRecordChanged, ct);
        }

        await context.SaveChangesAsync(ct);
        return SetReviewersResult.Done;
    }

    public async Task MaterializeDefaultStageAsync(Guid electionRoundId,
        Guid ngoId,
        ReviewStage defaultStage,
        CancellationToken ct)
    {
        var reviewedAt = defaultStage.Status == ReviewStatus.Reviewed ? "@now" : "NULL";
        foreach (var recordType in ReportRecordType.List)
        {
            await context.Database.ExecuteSqlRawAsync(
                $"""
                 INSERT INTO "ReviewWorkflows" ("Id", "ElectionRoundId", "MonitoringNgoId", "RecordType", "RecordId", "StageId", "StageChangedAt", "ReviewedAt")
                 SELECT gen_random_uuid(), @electionRoundId, @monitoringNgoId, @recordType, V."RecordId", @stageId, @now, {reviewedAt}
                 FROM "GetVisibleRecordIds"(@electionRoundId, @ngoId, @recordType) V
                 ON CONFLICT ("MonitoringNgoId", "RecordType", "RecordId") DO NOTHING
                 """,
                [
                    new NpgsqlParameter("electionRoundId", electionRoundId),
                    new NpgsqlParameter("ngoId", ngoId),
                    new NpgsqlParameter("monitoringNgoId", defaultStage.MonitoringNgoId),
                    new NpgsqlParameter("recordType", recordType.Value),
                    new NpgsqlParameter("stageId", defaultStage.Id),
                    new NpgsqlParameter("now", timeProvider.UtcNow)
                ],
                ct);
        }
    }

    public async Task AddActivityAsync(ReviewContext report, ReviewActivity activity, CancellationToken ct)
    {
        context.ReviewActivities.Add(activity);
        await context.NotifyAsync(activity,
            report.Reviewers.Select(x => x.UserId),
            UserNotificationReason.ReviewingRecordChanged,
            ct);
    }
}
