using Feature.Review.Services;

namespace Feature.Labels.Services;

public interface ILabelService
{
    /// <summary>The caller NGO's labels on the report, A to Z (ignoring case).</summary>
    Task<string[]> GetAsync(ReviewTarget target, CancellationToken ct);

    /// <summary>Every label on the NGO's records in the round, A to Z (ignoring case); empty outside the round.</summary>
    Task<string[]> ListAsync(Guid electionRoundId, Guid ngoId, CancellationToken ct);

    /// <summary>
    /// Replaces the report's labels in the caller's NGO. Sending the labels it has changes nothing; every real change is
    /// recorded once in the report's history and its reviewers are notified.
    /// </summary>
    Task SetAsync(ReviewContext report, IReadOnlyCollection<string> labels, ReviewActor actor, CancellationToken ct);
}

internal class LabelService(VoteMonitorContext context, IReviewService reviewService, ITimeProvider timeProvider)
    : ILabelService
{
    private static readonly StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

    public async Task<string[]> GetAsync(ReviewTarget target, CancellationToken ct) =>
        Sort(await context.RecordLabels
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == target.MonitoringNgoId
                        && x.RecordType == target.RecordType
                        && x.RecordId == target.RecordId)
            .Select(x => x.Label)
            .ToListAsync(ct));

    public async Task<string[]> ListAsync(Guid electionRoundId, Guid ngoId, CancellationToken ct)
    {
        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(electionRoundId, ngoId, ct);
        if (monitoringNgoId is null)
        {
            return [];
        }

        return Sort(await RoundLabelsAsync(electionRoundId, monitoringNgoId.Value, ct));
    }

    public async Task SetAsync(ReviewContext report,
        IReadOnlyCollection<string> labels,
        ReviewActor actor,
        CancellationToken ct)
    {
        var target = report.Target;
        var current = await context.RecordLabels
            .Where(x => x.MonitoringNgoId == target.MonitoringNgoId
                        && x.RecordType == target.RecordType
                        && x.RecordId == target.RecordId)
            .ToListAsync(ct);

        // A label the NGO already uses in the round keeps its spelling, so the same label reads the same everywhere.
        var known = (await RoundLabelsAsync(target.ElectionRoundId, target.MonitoringNgoId, ct))
            .GroupBy(x => x, IgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), IgnoreCase);
        var next = Normalize(labels).Select(x => known.GetValueOrDefault(x, x)).ToList();

        var removed = current.Where(x => !next.Contains(x.Label, IgnoreCase)).ToList();
        var added = next.Where(x => !current.Any(c => IgnoreCase.Equals(c.Label, x))).ToList();
        if (removed.Count == 0 && added.Count == 0)
        {
            return;
        }

        var now = timeProvider.UtcNow;
        context.RecordLabels.RemoveRange(removed);
        context.RecordLabels.AddRange(added.Select(x => RecordLabel.Create(target, x, actor.Id, now)));

        await reviewService.AddActivityAsync(report,
            ReviewActivity.LabelsChanged(target, actor, Sort(added), Sort(removed.Select(x => x.Label)), now),
            ct);

        await context.SaveChangesAsync(ct);
    }

    /// <summary>Trimmed, without blanks, each label once ignoring case (its first spelling kept).</summary>
    public static List<string> Normalize(IEnumerable<string?> labels) =>
        labels
            .Select(x => x?.Trim() ?? "")
            .Where(x => x.Length > 0)
            .Distinct(IgnoreCase)
            .ToList();

    /// <summary>The spellings of the NGO's labels in the round, the first used first.</summary>
    private async Task<List<string>> RoundLabelsAsync(Guid electionRoundId, Guid monitoringNgoId, CancellationToken ct)
    {
        var spellings = await context.RecordLabels
            .AsNoTracking()
            .Where(x => x.ElectionRoundId == electionRoundId && x.MonitoringNgoId == monitoringNgoId)
            .GroupBy(x => x.Label)
            .Select(x => new { Label = x.Key, FirstUsedAt = x.Min(l => l.CreatedAt) })
            .ToListAsync(ct);

        return spellings.OrderBy(x => x.FirstUsedAt).ThenBy(x => x.Label, StringComparer.Ordinal)
            .Select(x => x.Label)
            .ToList();
    }

    private static string[] Sort(IEnumerable<string> labels) =>
        labels
            .Distinct(IgnoreCase)
            .OrderBy(x => x, IgnoreCase)
            .ThenBy(x => x, StringComparer.Ordinal)
            .ToArray();
}
