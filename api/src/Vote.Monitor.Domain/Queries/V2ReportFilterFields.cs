using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Queries;

public static class V2ReportFilterFields
{
    public static readonly IReadOnlyDictionary<string, FilterField> FormSubmissions =
        new Dictionary<string, FilterField>
        {
            ["id"] = new("s.\"SubmissionId\"", "uuid",
                new HashSet<string> { FilterOps.Eq, FilterOps.In, FilterOps.NotIn }, FilterValue.Guid),
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
            ["followUpStatus"] = new("s.\"FollowUpStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<SubmissionFollowUpStatus>()),
            ["stageId"] = new("s.\"StageId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
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
            ["followUpStatus"] = new("s.\"FollowUpStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<CitizenReportFollowUpStatus>()),
            ["stageId"] = new("s.\"StageId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
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
            ["followUpStatus"] = new("s.\"FollowUpStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<IncidentReportFollowUpStatus>()),
            ["stageId"] = new("s.\"StageId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
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
            ["followUpStatus"] = new("s.\"FollowUpStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuickReportFollowUpStatus>()),
            ["stageId"] = new("s.\"StageId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["reviewStatus"] = new("s.\"ReviewStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<ReviewStatus>()),
            ["reviewerId"] = new("s.\"ReviewerIds\"", "uuid", FilterOperators.Array, FilterValue.Guid, IsArray: true),
            ["hasReviewer"] = new("(cardinality(s.\"ReviewerIds\") > 0)", "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["severity"] = new("s.\"Severity\"", "text", FilterOperators.Set,
                FilterValue.Enum<SeverityLevel>()),
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
