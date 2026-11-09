namespace Vote.Monitor.Domain.Entities.RecordLabelAggregate;

/// <summary>
/// Label columns and filters. The <c>Get*Entries</c> functions return the caller NGO's labels as <c>"Labels"</c>
/// (<c>text[]</c>, A to Z, empty when none); <c>"GetRecordLabels"(electionRoundId, ngoId, recordType, recordId)</c>
/// returns the same array for one record.
/// </summary>
public static class LabelSql
{
    /// <summary>
    /// A condition (starting with <c>AND</c>) keeping records carrying any of the given labels, ignoring case.
    /// Parameter: <c>@labels</c> (<c>text[]</c>); null or an empty array doesn't filter.
    /// </summary>
    public static string Filter(string labelsColumn) =>
        $"AND (@labels IS NULL OR cardinality(@labels::text[]) = 0 OR EXISTS (SELECT 1 FROM unnest({labelsColumn}) LBL(v) " +
        "WHERE lower(LBL.v) = ANY (SELECT lower(x) FROM unnest(@labels::text[]) x)))";

    /// <summary>The caller NGO's labels of a record, for queries with <c>@electionRoundId</c> and <c>@ngoId</c>.</summary>
    public static string LabelsOf(string idColumn, string recordType) =>
        $"\"GetRecordLabels\"(@electionRoundId, @ngoId, '{recordType}', {idColumn})";

    /// <summary>The <c>"Labels"</c> column of a <c>Get*Entries</c> row (alias <c>s</c>), lower-cased, for filters.</summary>
    public const string LowerCasedEntryLabels =
        "COALESCE((SELECT array_agg(lower(LBL.v)) FROM unnest(s.\"Labels\") LBL(v)), '{}'::text[])";

    /// <summary>Whether a <c>Get*Entries</c> row (alias <c>s</c>) carries no label.</summary>
    public const string EntryHasNoLabels = "COALESCE(cardinality(s.\"Labels\"), 0) = 0";
}
