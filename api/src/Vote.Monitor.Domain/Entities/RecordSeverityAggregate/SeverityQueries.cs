using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

/// <summary>A record's severity in one NGO and when it last changed (never: <c>NotAssessed</c>, no time).</summary>
public sealed record RecordSeverityValue(SeverityLevel Severity, DateTime? ChangedAt);

public static class SeverityQueries
{
    /// <summary>The NGO's severity of the record.</summary>
    public static async Task<RecordSeverityValue> GetRecordSeverityAsync(this VoteMonitorContext context,
        ReviewTarget target,
        CancellationToken ct)
    {
        var row = await context.RecordSeverities
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == target.MonitoringNgoId
                        && x.RecordType == target.RecordType
                        && x.RecordId == target.RecordId)
            .Select(x => new { x.Severity, x.ChangedAt })
            .FirstOrDefaultAsync(ct);
        return new RecordSeverityValue(row?.Severity ?? SeverityLevel.NotAssessed, row?.ChangedAt);
    }

    /// <summary>Removes every NGO's severity for a deleted record.</summary>
    public static Task DeleteSeverityAsync(this VoteMonitorContext context,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        context.RecordSeverities
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
}
