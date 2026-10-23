using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

public static class SeverityQueries
{
    /// <summary>Removes every NGO's severity for a deleted record.</summary>
    public static Task DeleteSeverityAsync(this VoteMonitorContext context,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        context.RecordSeverities
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
}
