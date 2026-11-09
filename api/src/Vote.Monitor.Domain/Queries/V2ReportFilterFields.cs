using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.RecordLabelAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Queries;

public static class V2ReportFilterFields
{
    /// <summary>
    /// A record's review stage: "is X" matches records in X and in X's sub-stages (the record's stage and its
    /// parent, as an array), so a parent stage filters its whole branch and a sub-stage only itself.
    /// </summary>
    private static readonly FilterField StageField = new(
        // NULLs dropped: "is not" must stay true for a top-level stage (x = ANY of a NULL is NULL, not false).
        "array_remove(ARRAY[s.\"StageId\", (SELECT rs.\"ParentStageId\" FROM \"ReviewStages\" rs WHERE rs.\"Id\" = s.\"StageId\")], NULL)",
        "uuid", FilterOperators.Set, FilterValue.Guid,
        EmptySql: "s.\"StageId\" IS NULL",
        IsArray: true);

    /// <summary>
    /// Whether its author changed the record's answers after the caller's NGO reviewed it (and it hasn't been
    /// reviewed again). The caller's workflow is the one in the row's stage: stages belong to one NGO.
    /// </summary>
    public static string ChangedAfterReviewSql(string idColumn) =>
        $"EXISTS (SELECT 1 FROM \"ReviewWorkflows\" w WHERE w.\"RecordId\" = {idColumn} AND w.\"StageId\" = s.\"StageId\" AND w.\"ChangedAfterReviewAt\" IS NOT NULL)";

    private static FilterField ChangedAfterReview(string idColumn) =>
        new($"({ChangedAfterReviewSql(idColumn)})", "boolean", FilterOperators.Boolean, FilterValue.Boolean);

    /// <summary>
    /// The caller NGO's follow-up status id for a row: its own row in "RecordFollowUps", else its default status.
    /// The caller's monitoring NGO is the one the row's review stage belongs to (stages belong to one NGO).
    /// </summary>
    public static string FollowUpStatusIdSql(string idColumn, string recordType)
    {
        const string callerNgo = "(SELECT RS.\"MonitoringNgoId\" FROM \"ReviewStages\" RS WHERE RS.\"Id\" = s.\"StageId\")";
        return $"COALESCE((SELECT RFU.\"FollowUpStatusId\" FROM \"RecordFollowUps\" RFU WHERE RFU.\"MonitoringNgoId\" = {callerNgo} " +
               $"AND RFU.\"RecordType\" = '{recordType}' AND RFU.\"RecordId\" = {idColumn}), " +
               $"(SELECT FUS.\"Id\" FROM \"FollowUpStatuses\" FUS WHERE FUS.\"MonitoringNgoId\" = {callerNgo} AND FUS.\"IsDefault\"))";
    }

    /// <summary>
    /// The name of an NGO's follow-up status for a record, for queries with @electionRoundId and @ngoId (exports): its
    /// own row's, else its default.
    /// </summary>
    public static string FollowUpStatusNameSql(string idColumn, string recordType)
    {
        const string ngo = "(SELECT MN.\"Id\" FROM \"MonitoringNgos\" MN WHERE MN.\"ElectionRoundId\" = @electionRoundId AND MN.\"NgoId\" = @ngoId)";
        return "(SELECT FUS.\"Name\" FROM \"FollowUpStatuses\" FUS WHERE FUS.\"Id\" = COALESCE(" +
               $"(SELECT RFU.\"FollowUpStatusId\" FROM \"RecordFollowUps\" RFU WHERE RFU.\"MonitoringNgoId\" = {ngo} AND RFU.\"RecordType\" = '{recordType}' AND RFU.\"RecordId\" = {idColumn}), " +
               $"(SELECT D.\"Id\" FROM \"FollowUpStatuses\" D WHERE D.\"MonitoringNgoId\" = {ngo} AND D.\"IsDefault\")))";
    }

    /// <summary>
    /// A join adding one monitoring NGO's follow-up status for each row (its own row's, else its default) as
    /// "FollowUpStatusId", "FollowUpStatusName", "FollowUpStatusColor", "FollowUpStatusIcon" (alias FU): platform lists,
    /// where the NGO is the one that owns the record.
    /// </summary>
    public static string FollowUpOfNgoJoinSql(string idColumn, string recordType, string monitoringNgoIdColumn) =>
        "LEFT JOIN LATERAL (SELECT F.\"Id\" AS \"FollowUpStatusId\", F.\"Name\" AS \"FollowUpStatusName\", F.\"Color\" AS \"FollowUpStatusColor\", F.\"Icon\" AS \"FollowUpStatusIcon\" " +
        "FROM \"FollowUpStatuses\" F WHERE F.\"Id\" = COALESCE(" +
        $"(SELECT RFU.\"FollowUpStatusId\" FROM \"RecordFollowUps\" RFU WHERE RFU.\"MonitoringNgoId\" = {monitoringNgoIdColumn} AND RFU.\"RecordType\" = '{recordType}' AND RFU.\"RecordId\" = {idColumn}), " +
        $"(SELECT D.\"Id\" FROM \"FollowUpStatuses\" D WHERE D.\"MonitoringNgoId\" = {monitoringNgoIdColumn} AND D.\"IsDefault\"))) FU ON TRUE";

    /// <summary>The caller NGO's follow-up status (its id), from its own statuses.</summary>
    private static FilterField FollowUp(string idColumn, string recordType) =>
        new($"({FollowUpStatusIdSql(idColumn, recordType)})", "uuid", FilterOperators.Set, FilterValue.Guid);

    /// <summary>
    /// The caller NGO's labels on a record (the row's "Labels"), an array field matched ignoring case: "in" has any of
    /// the labels, "notIn" none of them, "isEmpty" / "isNotEmpty" no label / at least one.
    /// </summary>
    private static readonly FilterField LabelsField = new(
        LabelSql.LowerCasedEntryLabels,
        "text",
        new HashSet<string> { FilterOps.In, FilterOps.NotIn, FilterOps.IsEmpty, FilterOps.IsNotEmpty },
        value => (FilterValue.String(value) as string)?.Trim().ToLowerInvariant(),
        EmptySql: LabelSql.EntryHasNoLabels,
        IsArray: true);

    public static readonly IReadOnlyDictionary<string, FilterField> FormSubmissions =
        new Dictionary<string, FilterField>
        {
            ["id"] = new("s.\"SubmissionId\"", "uuid",
                new HashSet<string> { FilterOps.Eq, FilterOps.In, FilterOps.NotIn }, FilterValue.Guid),
            // The list's search box: the observer's name or email, or the submission id (any part, any case).
            ["searchText"] = new(
                "concat_ws(' ', s.\"DisplayName\", s.\"Email\"::text, s.\"SubmissionId\"::text)", "text",
                new HashSet<string> { FilterOps.Contains }, FilterValue.String, IgnoreCase: true),
            ["formId"] = new("s.\"FormId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["monitoringObserverId"] = new("s.\"MonitoringObserverId\"", "uuid", FilterOperators.Set,
                FilterValue.Guid),
            ["coalitionMemberId"] = new("s.\"NgoId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["tags"] = new("COALESCE(s.\"Tags\", '{}')", "text", FilterOperators.Array, FilterValue.String,
                IsArray: true),
            ["pollingStationId"] = new("s.\"PollingStationId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["level1"] = new("s.\"Level1\"", "text", FilterOperators.Set, FilterValue.String),
            ["level2"] = new("s.\"Level2\"", "text", FilterOperators.Set, FilterValue.String),
            ["level3"] = new("s.\"Level3\"", "text", FilterOperators.Set, FilterValue.String),
            ["level4"] = new("s.\"Level4\"", "text", FilterOperators.Set, FilterValue.String),
            ["level5"] = new("s.\"Level5\"", "text", FilterOperators.Set, FilterValue.String),
            ["pollingStationNumber"] = new("s.\"Number\"", "text", FilterOperators.Text, FilterValue.String),
            ["submittedAt"] = new("s.\"TimeSubmitted\"", "timestamptz", FilterOperators.Comparison,
                FilterValue.UtcDateTimeOffset),
            ["hasFlaggedAnswers"] = new("(s.\"NumberOfFlaggedAnswers\" > 0)", "boolean",
                FilterOperators.Boolean, FilterValue.Boolean),
            ["hasNotes"] = new("s.\"HasNotes\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasComments"] = new("s.\"HasComments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasAttachments"] = new("s.\"HasAttachments\"", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["followUpStatus"] = FollowUp("s.\"SubmissionId\"", "FormSubmission"),
            ["stageId"] = StageField,
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["changedAfterReview"] = ChangedAfterReview("s.\"SubmissionId\""),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
            ["labels"] = LabelsField,
            ["formType"] = new("s.\"FormType\"", "text", FilterOperators.Set, FilterValue.Enum<FormType>()),
            ["formStatus"] = new("s.\"FormStatus\"", "text", FilterOperators.Set, FilterValue.Enum<FormStatus>()),
            ["questionsAnswered"] = new("s.\"QuestionsAnswered\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuestionsAnsweredFilter>())
        };

    public static readonly IReadOnlyDictionary<string, FilterField> CitizenReports =
        new Dictionary<string, FilterField>
        {
            ["id"] = new("s.\"Id\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["formId"] = new("s.\"FormId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["formStatus"] = new("s.\"FormStatus\"", "text", FilterOperators.Set, FilterValue.Enum<FormStatus>()),
            ["locationId"] = new("s.\"LocationId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["level1"] = new("s.\"Level1\"", "text", FilterOperators.Set, FilterValue.String),
            ["level2"] = new("s.\"Level2\"", "text", FilterOperators.Set, FilterValue.String),
            ["level3"] = new("s.\"Level3\"", "text", FilterOperators.Set, FilterValue.String),
            ["level4"] = new("s.\"Level4\"", "text", FilterOperators.Set, FilterValue.String),
            ["level5"] = new("s.\"Level5\"", "text", FilterOperators.Set, FilterValue.String),
            ["submittedAt"] = new("s.\"TimeSubmitted\"", "timestamptz", FilterOperators.Comparison,
                FilterValue.UtcDateTimeOffset),
            ["hasFlaggedAnswers"] = new("(s.\"NumberOfFlaggedAnswers\" > 0)", "boolean",
                FilterOperators.Boolean, FilterValue.Boolean),
            ["followUpStatus"] = FollowUp("s.\"Id\"", "CitizenReport"),
            // The record's own fixed value (older query parameters); the NGO's own status is "followUpStatus".
            ["recordFollowUpStatus"] = new("s.\"FollowUpStatus\"", "text", FilterOperators.Set, FilterValue.Enum<CitizenReportFollowUpStatus>()),
            ["stageId"] = StageField,
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
            ["labels"] = LabelsField,
            ["hasNotes"] = new("s.\"HasNotes\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasComments"] = new("s.\"HasComments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasAttachments"] = new("s.\"HasAttachments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["questionsAnswered"] = new("s.\"QuestionsAnswered\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuestionsAnsweredFilter>())
        };

    public static readonly IReadOnlyDictionary<string, FilterField> IncidentReports =
        new Dictionary<string, FilterField>
        {
            ["id"] = new("s.\"Id\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["formId"] = new("s.\"FormId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["formStatus"] = new("s.\"FormStatus\"", "text", FilterOperators.Set, FilterValue.Enum<FormStatus>()),
            ["monitoringObserverId"] = new("s.\"MonitoringObserverId\"", "uuid", FilterOperators.Set,
                FilterValue.Guid),
            ["coalitionMemberId"] = new("s.\"NgoId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["tags"] = new("COALESCE(s.\"Tags\", '{}')", "text", FilterOperators.Array, FilterValue.String,
                IsArray: true),
            ["pollingStationId"] = new("s.\"PollingStationId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["level1"] = new("s.\"Level1\"", "text", FilterOperators.Set, FilterValue.String),
            ["level2"] = new("s.\"Level2\"", "text", FilterOperators.Set, FilterValue.String),
            ["level3"] = new("s.\"Level3\"", "text", FilterOperators.Set, FilterValue.String),
            ["level4"] = new("s.\"Level4\"", "text", FilterOperators.Set, FilterValue.String),
            ["level5"] = new("s.\"Level5\"", "text", FilterOperators.Set, FilterValue.String),
            ["pollingStationNumber"] = new("s.\"Number\"", "text", FilterOperators.Text, FilterValue.String),
            ["submittedAt"] = new("s.\"TimeSubmitted\"", "timestamptz", FilterOperators.Comparison,
                FilterValue.UtcDateTimeOffset),
            ["hasFlaggedAnswers"] = new("(s.\"NumberOfFlaggedAnswers\" > 0)", "boolean",
                FilterOperators.Boolean, FilterValue.Boolean),
            ["followUpStatus"] = FollowUp("s.\"Id\"", "IncidentReport"),
            // The record's own fixed value (older query parameters); the NGO's own status is "followUpStatus".
            ["recordFollowUpStatus"] = new("s.\"FollowUpStatus\"", "text", FilterOperators.Set, FilterValue.Enum<IncidentReportFollowUpStatus>()),
            ["stageId"] = StageField,
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["changedAfterReview"] = ChangedAfterReview("s.\"Id\""),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
            ["labels"] = LabelsField,
            ["locationType"] = new("s.\"LocationType\"", "text", FilterOperators.Set,
                IncidentReportLocationTypeValue),
            ["isCompleted"] = new("s.\"IsCompleted\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasNotes"] = new("s.\"HasNotes\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasComments"] = new("s.\"HasComments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasAttachments"] = new("s.\"HasAttachments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["questionsAnswered"] = new("s.\"QuestionsAnswered\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuestionsAnsweredFilter>()),
            ["formType"] = new("s.\"FormType\"", "text", FilterOperators.Set, FilterValue.Enum<FormType>())
        };

    public static readonly IReadOnlyDictionary<string, FilterField> QuickReports =
        new Dictionary<string, FilterField>
        {
            ["id"] = new("s.\"Id\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["monitoringObserverId"] = new("s.\"MonitoringObserverId\"", "uuid", FilterOperators.Set,
                FilterValue.Guid),
            ["coalitionMemberId"] = new("s.\"NgoId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["pollingStationId"] = new("s.\"PollingStationId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["level1"] = new("s.\"Level1\"", "text", FilterOperators.Set, FilterValue.String),
            ["level2"] = new("s.\"Level2\"", "text", FilterOperators.Set, FilterValue.String),
            ["level3"] = new("s.\"Level3\"", "text", FilterOperators.Set, FilterValue.String),
            ["level4"] = new("s.\"Level4\"", "text", FilterOperators.Set, FilterValue.String),
            ["level5"] = new("s.\"Level5\"", "text", FilterOperators.Set, FilterValue.String),
            ["pollingStationNumber"] = new("s.\"Number\"", "text", FilterOperators.Text, FilterValue.String),
            ["submittedAt"] = new("s.\"TimeSubmitted\"", "timestamptz", FilterOperators.Comparison,
                FilterValue.UtcDateTimeOffset),
            ["followUpStatus"] = FollowUp("s.\"Id\"", "QuickReport"),
            ["stageId"] = StageField,
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
            ["labels"] = LabelsField,
            ["locationType"] = new("s.\"LocationType\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuickReportLocationType>()),
            ["incidentCategory"] = new("s.\"IncidentCategory\"", "text", FilterOperators.Set,
                FilterValue.Enum<IncidentCategory>()),
            ["hasComments"] = new("s.\"HasComments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasAttachments"] = new("s.\"HasAttachments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean)
        };

    // Also accepts the legacy "OtherLocation", which was renamed to NotRelatedToAPollingStation.
    private static object? IncidentReportLocationTypeValue(JsonElement value) =>
        value.ValueKind == JsonValueKind.String && value.GetString() == IncidentReportLocationType.LegacyOtherLocation
            ? IncidentReportLocationType.NotRelatedToAPollingStation.Value
            : FilterValue.Enum<IncidentReportLocationType>()(value);
}
