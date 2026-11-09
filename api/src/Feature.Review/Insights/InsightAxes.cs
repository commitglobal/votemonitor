using Vote.Monitor.Domain.Queries;

namespace Feature.Review.Insights;

/// <summary>
/// One way to split a report list's rows (alias <c>s</c>, the <c>Get*Entries</c> functions). <c>{a}</c> in the SQL is
/// the axis's own alias, for the axes that need a join: follow-up statuses, and reviewers, tags and labels, which count
/// a record once for each of its values (records with none under a null key).
/// </summary>
public sealed record InsightAxis(string KeySql, string? LabelSql = null, string? Join = null, bool IsTime = false)
{
    public string Key(string alias) => KeySql.Replace("{a}", alias);
    public string Label(string alias) => (LabelSql ?? "NULL").Replace("{a}", alias);
    public string JoinSql(string alias) => Join?.Replace("{a}", alias) ?? "";
}

public static class InsightAxes
{
    public const string ReviewStatus = "reviewStatus";
    public const string Stage = "stage";
    public const string Severity = "severity";
    public const string FollowUpStatus = "followUpStatus";
    public const string Reviewer = "reviewer";
    public const string Form = "form";
    public const string FormType = "formType";
    public const string Observer = "observer";
    public const string CoalitionMember = "coalitionMember";
    public const string Tag = "tag";
    public const string Label = "label";
    public const string IncidentCategory = "incidentCategory";
    public const string LocationType = "locationType";
    public const string HasFlaggedAnswers = "hasFlaggedAnswers";
    public const string SubmittedDay = "submittedDay";
    public const string SubmittedHour = "submittedHour";

    /// <summary>Separates the names of a place's levels in a level axis's key.</summary>
    public const string LevelSeparator = "\u001f";

    public static IReadOnlyDictionary<string, InsightAxis> For(ReportRecordType recordType)
    {
        var id = recordType == ReportRecordType.FormSubmission ? "s.\"SubmissionId\"" : "s.\"Id\"";
        var axes = new Dictionary<string, InsightAxis>
        {
            [ReviewStatus] = new("s.\"ReviewStatus\""),
            [Stage] = new("s.\"StageId\"::text", "s.\"StageName\""),
            [Severity] = new("s.\"Severity\""),
            [FollowUpStatus] = new("{a}.\"Id\"::text", "{a}.\"Name\"",
                $"LEFT JOIN \"FollowUpStatuses\" {{a}} ON {{a}}.\"Id\" = {V2ReportFilterFields.FollowUpStatusIdSql(id, recordType.Value)}"),
            [Reviewer] = new("{a}.v::text", "(SELECT U.\"DisplayName\" FROM \"AspNetUsers\" U WHERE U.\"Id\" = {a}.v)",
                "LEFT JOIN LATERAL UNNEST(COALESCE(NULLIF(s.\"ReviewerIds\", '{}'), ARRAY[NULL::uuid])) AS {a}(v) ON TRUE"),
            // The caller NGO's labels: a record counts once under each of its labels, records without one under null.
            [Label] = new("{a}.v", "{a}.v",
                "LEFT JOIN LATERAL UNNEST(COALESCE(NULLIF(s.\"Labels\", '{}'), ARRAY[NULL::text])) AS {a}(v) ON TRUE"),
            [SubmittedDay] = new("TO_CHAR(s.\"TimeSubmitted\" AT TIME ZONE @timeZone, 'YYYY-MM-DD')", IsTime: true),
            [SubmittedHour] = new("TO_CHAR(DATE_TRUNC('hour', s.\"TimeSubmitted\" AT TIME ZONE @timeZone), 'YYYY-MM-DD\"T\"HH24:MI')", IsTime: true),
        };

        for (var level = 1; level <= 5; level++)
        {
            axes[$"level{level}"] = new(LevelKey(level));
        }

        if (recordType != ReportRecordType.QuickReport)
        {
            var language = recordType == ReportRecordType.FormSubmission ? "s.\"DefaultLanguage\"" : "s.\"FormDefaultLanguage\"";
            axes[Form] = new("s.\"FormId\"::text", $"CONCAT_WS(' – ', s.\"FormCode\", s.\"FormName\" ->> {language})");
            axes[HasFlaggedAnswers] = new("(s.\"NumberOfFlaggedAnswers\" > 0)::text");
        }

        if (recordType != ReportRecordType.CitizenReport)
        {
            axes[Observer] = new("s.\"MonitoringObserverId\"::text", "s.\"DisplayName\"");
            axes[CoalitionMember] = new("s.\"NgoId\"::text", "s.\"NgoName\"");
            axes[Tag] = new("{a}.v", "{a}.v",
                "LEFT JOIN LATERAL UNNEST(COALESCE(NULLIF(s.\"Tags\", '{}'), ARRAY[NULL::text])) AS {a}(v) ON TRUE");
        }

        if (recordType == ReportRecordType.FormSubmission || recordType == ReportRecordType.IncidentReport)
        {
            axes[FormType] = new("s.\"FormType\"");
        }

        if (recordType == ReportRecordType.QuickReport)
        {
            axes[IncidentCategory] = new("s.\"IncidentCategory\"");
        }

        if (recordType == ReportRecordType.QuickReport || recordType == ReportRecordType.IncidentReport)
        {
            axes[LocationType] = new("s.\"LocationType\"");
        }

        return axes;
    }

    /// <summary>A place down to <paramref name="level"/> (its levels' names joined); null when it has no name there.</summary>
    private static string LevelKey(int level)
    {
        var names = string.Join(", ", Enumerable.Range(1, level).Select(n => $"s.\"Level{n}\""));
        return $"CASE WHEN NULLIF(s.\"Level{level}\", '') IS NULL THEN NULL ELSE CONCAT_WS(E'\\x1f', {names}) END";
    }
}

/// <summary>What the bars measure: SQL aggregates over a report list's rows.</summary>
public static class InsightMetrics
{
    public const string Records = "records";
    public const string FlaggedAnswers = "flaggedAnswers";
    public const string MediaFiles = "mediaFiles";
    public const string Notes = "notes";
    public const string Comments = "comments";
    public const string Observers = "observers";
    public const string PollingStations = "pollingStations";

    public static IReadOnlyDictionary<string, string> For(ReportRecordType recordType)
    {
        var metrics = new Dictionary<string, string>
        {
            [Records] = "COUNT(1)",
            [Comments] = "COALESCE(SUM(s.\"CommentsCount\"), 0)",
            [MediaFiles] = recordType == ReportRecordType.QuickReport
                ? "COALESCE(SUM(s.\"NumberOfAttachments\"), 0)"
                : "COALESCE(SUM(s.\"MediaFilesCount\"), 0)",
        };

        if (recordType != ReportRecordType.QuickReport)
        {
            metrics[FlaggedAnswers] = "COALESCE(SUM(s.\"NumberOfFlaggedAnswers\"), 0)";
            metrics[Notes] = "COALESCE(SUM(s.\"NotesCount\"), 0)";
        }

        if (recordType != ReportRecordType.CitizenReport)
        {
            metrics[Observers] = "COUNT(DISTINCT s.\"MonitoringObserverId\")";
            metrics[PollingStations] = "COUNT(DISTINCT s.\"PollingStationId\")";
        }

        return metrics;
    }
}
