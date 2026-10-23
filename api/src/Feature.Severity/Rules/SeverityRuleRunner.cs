using Feature.Review.Rules;
using Feature.Review.Services;
using Feature.Severity.Services;

namespace Feature.Severity.Rules;

public interface ISeverityRuleRunner
{
    /// <summary>
    /// Raises the report's severity to the highest of the NGO's severity rules it matches, when that is higher than
    /// its current severity; rules never lower it. The change is attributed to the rule that gave that severity (the
    /// first by name on a tie), recorded in the report's history and notified to its reviewers.
    /// </summary>
    Task RunAsync(ReviewContext report, CancellationToken ct);
}

internal class SeverityRuleRunner(
    VoteMonitorContext context,
    IReviewService reviewService,
    ISeverityService severityService,
    ITimeProvider timeProvider) : ISeverityRuleRunner
{
    public async Task RunAsync(ReviewContext report, CancellationToken ct)
    {
        var rules = (await context.SeverityRules
                .AsNoTracking()
                .Where(x => x.MonitoringNgoId == report.Target.MonitoringNgoId)
                .ToListAsync(ct))
            .Where(x => x.RecordTypes.Contains(report.Target.RecordType))
            .ToList();

        var matches = await context.MatchAsync(report, rules, ct);
        var rule = rules
            .Where(x => matches.Contains(x.Id))
            .OrderByDescending(x => x.Severity.Level)
            .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        var current = await severityService.GetAsync(report.Target, ct);
        var from = current?.Severity ?? SeverityLevel.None;
        if (rule is null || rule.Severity.Level <= from.Level)
        {
            return;
        }

        var now = timeProvider.UtcNow;
        if (current is null)
        {
            context.RecordSeverities.Add(RecordSeverity.Create(report.Target.ElectionRoundId,
                report.Target.MonitoringNgoId,
                report.Target.RecordType,
                report.Target.RecordId,
                rule.Severity,
                changedBy: null,
                now));
        }
        else
        {
            current.Change(rule.Severity, changedBy: null, now);
        }

        await reviewService.AddActivityAsync(report,
            ReviewActivity.SeverityChanged(report.Target, rule, from, rule.Severity, now),
            ct);

        await context.SaveChangesAsync(ct);
    }
}
