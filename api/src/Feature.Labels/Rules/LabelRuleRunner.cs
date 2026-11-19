using Feature.Labels.Services;
using Feature.Review.Rules;
using Feature.Review.Services;

namespace Feature.Labels.Rules;

public interface ILabelRuleRunner
{
    /// <summary>
    /// Adds the labels of the NGO's label rules the report matches (by name) that it doesn't carry yet, ignoring case,
    /// up to a record's limit. Rules never remove a label. Each rule that added a label is recorded in the report's
    /// history, attributed to it; the report's reviewers are notified once.
    /// </summary>
    Task RunAsync(ReviewContext report, CancellationToken ct);
}

internal class LabelRuleRunner(VoteMonitorContext context, ILabelService labelService) : ILabelRuleRunner
{
    public async Task RunAsync(ReviewContext report, CancellationToken ct)
    {
        var rules = (await context.LabelRules
                .AsNoTracking()
                .Where(x => x.MonitoringNgoId == report.Target.MonitoringNgoId)
                .ToListAsync(ct))
            .Where(x => x.RecordTypes.Contains(report.Target.RecordType))
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matches = await context.MatchAsync(report, rules, ct);
        var matching = rules.Where(x => matches.Contains(x.Id)).ToList();
        if (matching.Count == 0)
        {
            return;
        }

        await labelService.AddByRulesAsync(report, matching, ct);
    }
}
