using Feature.Review.Rules;
using Feature.Review.Services;

namespace Feature.Review.FollowUpRules;

public interface IFollowUpRuleRunner
{
    /// <summary>
    /// While the report still has the NGO's default follow-up status, moves it to the status of the first of the NGO's
    /// follow-up rules (by name) it matches whose status isn't the default. Recorded in the report's history,
    /// attributed to that rule, and notified to its reviewers. A status a person or an earlier rule chose stays.
    /// </summary>
    Task RunAsync(ReviewContext report, CancellationToken ct);
}

internal class FollowUpRuleRunner(VoteMonitorContext context, IFollowUpService followUpService) : IFollowUpRuleRunner
{
    public async Task RunAsync(ReviewContext report, CancellationToken ct)
    {
        var current = await followUpService.GetCurrentAsync(report.Target, ct);
        if (!current.IsDefault)
        {
            return;
        }

        var rules = (await context.FollowUpRules
                .AsNoTracking()
                .Where(x => x.MonitoringNgoId == report.Target.MonitoringNgoId && x.FollowUpStatusId != current.Id)
                .ToListAsync(ct))
            .Where(x => x.RecordTypes.Contains(report.Target.RecordType))
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matches = await context.MatchAsync(report, rules, ct);
        var rule = rules.FirstOrDefault(x => matches.Contains(x.Id));
        if (rule is null)
        {
            return;
        }

        await followUpService.SetByRuleAsync(report, rule, ct);
    }
}
