using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordLabelAggregate;

public static class LabelQueries
{
    /// <summary>The NGO's labels on the record, sorted ignoring case.</summary>
    public static async Task<string[]> GetRecordLabelsAsync(this VoteMonitorContext context,
        ReviewTarget target,
        CancellationToken ct)
    {
        var labels = await context.RecordLabels
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == target.MonitoringNgoId
                        && x.RecordType == target.RecordType
                        && x.RecordId == target.RecordId)
            .Select(x => x.Label)
            .ToListAsync(ct);
        return labels
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>Removes every NGO's labels from a deleted record.</summary>
    public static Task DeleteLabelsAsync(this VoteMonitorContext context,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        context.RecordLabels
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
}
