using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordLabelAggregate;

public static class LabelQueries
{
    /// <summary>Removes every NGO's labels from a deleted record.</summary>
    public static Task DeleteLabelsAsync(this VoteMonitorContext context,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        context.RecordLabels
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
}
