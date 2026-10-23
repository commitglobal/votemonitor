namespace Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

/// <summary>
/// Severity filtering and sorting for queries that join <c>"GetRecordSeverity"</c> (the V1 lists) or read the
/// <c>Get*Entries</c> functions, which return the same <c>"Severity"</c> column. Sort by level with
/// <c>"SeverityRank"(severity)</c>.
/// </summary>
public static class SeveritySql
{
    /// <summary>
    /// A condition (starting with <c>AND</c>) keeping records with any of the given severities. Parameter:
    /// <c>@severities</c> (<c>text[]</c>); an empty array doesn't filter.
    /// </summary>
    public static string Filter(string column = "SV.\"Severity\"") =>
        $"AND (@severities IS NULL OR cardinality(@severities) = 0 OR {column} = ANY(@severities))";

    /// <summary>Whether a list's <c>SortColumnName</c> asks to sort by severity.</summary>
    public static bool IsSortColumn(string? sortColumnName) =>
        string.Equals(sortColumnName, "severity", StringComparison.InvariantCultureIgnoreCase);
}
