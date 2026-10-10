using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

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
            ["formType"] = new("s.\"FormType\"", "text", FilterOperators.Set, FilterValue.Enum<FormType>()),
            ["questionsAnswered"] = new("s.\"QuestionsAnswered\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuestionsAnsweredFilter>())
        };

    public static readonly IReadOnlyDictionary<string, FilterField> CitizenReports =
        new Dictionary<string, FilterField>
        {
            ["id"] = new("s.\"Id\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["formId"] = new("s.\"FormId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
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
            ["monitoringObserverId"] = new("s.\"MonitoringObserverId\"", "uuid", FilterOperators.Set,
                FilterValue.Guid),
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
            ["locationType"] = new("s.\"LocationType\"", "text", FilterOperators.Set,
                FilterValue.Enum<IncidentReportLocationType>()),
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
            ["pollingStationId"] = new("s.\"PollingStationId\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["level1"] = new("s.\"Level1\"", "text", FilterOperators.Set, FilterValue.String),
            ["level2"] = new("s.\"Level2\"", "text", FilterOperators.Set, FilterValue.String),
            ["level3"] = new("s.\"Level3\"", "text", FilterOperators.Set, FilterValue.String),
            ["level4"] = new("s.\"Level4\"", "text", FilterOperators.Set, FilterValue.String),
            ["level5"] = new("s.\"Level5\"", "text", FilterOperators.Set, FilterValue.String),
            ["submittedAt"] = new("s.\"TimeSubmitted\"", "timestamptz", FilterOperators.Comparison,
                FilterValue.UtcDateTimeOffset),
            ["followUpStatus"] = new("s.\"FollowUpStatus\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuickReportFollowUpStatus>()),
            ["locationType"] = new("s.\"LocationType\"", "text", FilterOperators.Set,
                FilterValue.Enum<QuickReportLocationType>()),
            ["incidentCategory"] = new("s.\"IncidentCategory\"", "text", FilterOperators.Set,
                FilterValue.Enum<IncidentCategory>()),
            ["hasComments"] = new("s.\"HasComments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean),
            ["hasAttachments"] = new("s.\"HasAttachments\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean)
        };
}
