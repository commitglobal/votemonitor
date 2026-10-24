using Feature.Review.Services;

namespace Feature.Severity.Services;

public interface ISeverityService
{
    /// <summary>The caller NGO's severity row for the report, or null when it never set one (severity None).</summary>
    Task<RecordSeverity?> GetAsync(ReviewTarget target, CancellationToken ct);

    /// <summary>
    /// NGO admins and staff change any report's severity. Setting the current value changes nothing; every real
    /// change is recorded in the report's history and its reviewers are notified.
    /// </summary>
    Task SetAsync(ReviewContext report,
        SeverityLevel severity,
        ReviewActor actor,
        CancellationToken ct);
}

internal class SeverityService(VoteMonitorContext context, IReviewService reviewService, ITimeProvider timeProvider)
    : ISeverityService
{
    public Task<RecordSeverity?> GetAsync(ReviewTarget target, CancellationToken ct) =>
        context.RecordSeverities.FirstOrDefaultAsync(x => x.MonitoringNgoId == target.MonitoringNgoId
                                                          && x.RecordType == target.RecordType
                                                          && x.RecordId == target.RecordId, ct);

    public async Task SetAsync(ReviewContext report,
        SeverityLevel severity,
        ReviewActor actor,
        CancellationToken ct)
    {
        var now = timeProvider.UtcNow;
        var current = await GetAsync(report.Target, ct);
        var from = current?.Severity ?? SeverityLevel.None;
        if (from == severity)
        {
            return;
        }

        if (current is null)
        {
            context.RecordSeverities.Add(RecordSeverity.Create(report.Target.ElectionRoundId,
                report.Target.MonitoringNgoId,
                report.Target.RecordType,
                report.Target.RecordId,
                severity,
                actor.Id,
                now));
        }
        else
        {
            current.Change(severity, actor.Id, now);
        }

        await reviewService.AddActivityAsync(report,
            ReviewActivity.SeverityChanged(report.Target, actor, from, severity, now),
            ct);

        await context.SaveChangesAsync(ct);
    }
}
