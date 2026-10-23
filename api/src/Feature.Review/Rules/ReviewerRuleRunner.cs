using Feature.Review.Services;

namespace Feature.Review.Rules;

public interface IReviewerRuleRunner
{
    /// <summary>
    /// Adds the reviewers of the NGO's reviewer rules that the report matches and that trigger on the stage it just
    /// entered. Rules only add: reviewers already on the report stay, users who are no longer active admins or staff
    /// are skipped, and the report never gets more than <see cref="ReviewService.MaxReviewers"/> reviewers. A user
    /// several rules add is attributed to the first rule by name.
    /// </summary>
    Task RunAsync(ReviewContext report, ReviewStage stage, CancellationToken ct);
}

internal class ReviewerRuleRunner(VoteMonitorContext context, ITimeProvider timeProvider) : IReviewerRuleRunner
{
    public async Task RunAsync(ReviewContext report, ReviewStage stage, CancellationToken ct)
    {
        var rules = (await context.ReviewerRules
                .AsNoTracking()
                .Where(x => x.MonitoringNgoId == report.Target.MonitoringNgoId && x.StageIds.Contains(stage.Id))
                .ToListAsync(ct))
            .Where(x => x.RecordTypes.Contains(report.Target.RecordType))
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matches = await context.MatchAsync(report, rules, ct);
        var current = report.Reviewers.Select(x => x.UserId).ToList();
        var candidates = rules
            .Where(x => matches.Contains(x.Id))
            .SelectMany(rule => rule.ReviewerIds.Select(userId => (UserId: userId, Rule: rule)))
            .Where(x => !current.Contains(x.UserId))
            .DistinctBy(x => x.UserId)
            .ToList();

        var assignable = await context.GetAssignableUsersAsync(report.NgoId,
            candidates.Select(x => x.UserId).ToList(),
            ct);
        var added = candidates
            .Where(x => assignable.ContainsKey(x.UserId))
            .Take(Math.Max(0, ReviewService.MaxReviewers - current.Count))
            .ToList();
        if (added.Count == 0)
        {
            return;
        }

        var now = timeProvider.UtcNow;
        foreach (var (userId, rule) in added)
        {
            context.Reviewers.Add(Reviewer.Create(report.Target.ElectionRoundId,
                report.Target.MonitoringNgoId,
                report.Target.RecordType,
                report.Target.RecordId,
                userId,
                now,
                assignedBy: null));

            var activity = ReviewActivity.ReviewerAdded(report.Target, rule, userId, assignable[userId], now);
            context.ReviewActivities.Add(activity);
            await context.NotifyAsync(activity, [userId], UserNotificationReason.AddedAsReviewer, ct);
            await context.NotifyAsync(activity, current, UserNotificationReason.ReviewingRecordChanged, ct);
        }

        await context.SaveChangesAsync(ct);
    }
}
