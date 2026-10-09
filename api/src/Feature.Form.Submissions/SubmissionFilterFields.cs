using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Form.Submissions;

public static class SubmissionFilterFields
{
    private static readonly HashSet<string> Comparison =
    [
        FilterOps.Eq, FilterOps.Ne,
        FilterOps.Lt, FilterOps.Lte,
        FilterOps.Gt, FilterOps.Gte,
        FilterOps.Between
    ];

    private static readonly HashSet<string> Set =
    [
        FilterOps.Eq, FilterOps.Ne,
        FilterOps.In, FilterOps.NotIn,
        FilterOps.IsEmpty, FilterOps.IsNotEmpty
    ];

    private static readonly HashSet<string> Text =
    [
        FilterOps.Eq, FilterOps.Ne,
        FilterOps.Contains, FilterOps.NotContains,
        FilterOps.IsEmpty, FilterOps.IsNotEmpty
    ];

    private static readonly HashSet<string> Boolean =
    [
        FilterOps.Eq, FilterOps.IsEmpty, FilterOps.IsNotEmpty
    ];

    public static readonly IReadOnlyDictionary<string, FilterField> All =
        new Dictionary<string, FilterField>
        {
            ["id"] = new(
                "s.\"Id\"",
                "uuid",
                new HashSet<string> { FilterOps.Eq, FilterOps.In, FilterOps.NotIn },
                FilterValue.Guid),
            ["formId"] = new(
                "s.\"FormId\"",
                "uuid",
                Set,
                FilterValue.Guid),
            ["monitoringObserverId"] = new(
                "s.\"MonitoringObserverId\"",
                "uuid",
                Set,
                FilterValue.Guid),
            ["level1"] = new(
                "s.\"Level1\"",
                "text",
                Set,
                FilterValue.String),            
            ["level2"] = new(
                "s.\"Level2\"",
                "text",
                Set,
                FilterValue.String),
            ["level3"] = new(
                "s.\"Level3\"",
                "text",
                Set,
                FilterValue.String),
            ["level4"] = new(
                "s.\"Level4\"",
                "text",
                Set,
                FilterValue.String),
            ["level5"] = new(
                "s.\"Level5\"",
                "text",
                Set,
                FilterValue.String),
            ["pollingStationNumber"] = new(
                "s.\"Number\"",
                "text",
                Text,
                FilterValue.String),
            ["submittedAt"] = new(
                "s.\"TimeSubmitted\"",
                "timestamptz",
                Comparison,
                FilterValue.UtcDateTimeOffset),
            ["hasFlaggedAnswers"] = new(
                "s.\"HasFlaggedAnswers\"",
                "boolean",
                Boolean,
                FilterValue.Boolean),
            ["hasNotes"] = new(
                "s.\"HasNotes\"",
                "boolean",
                Boolean,
                FilterValue.Boolean),
            ["hasComments"] = new(
                "s.\"HasComments\"",
                "boolean",
                Boolean,
                FilterValue.Boolean),
            ["hasAttachments"] = new(
                "s.\"HasAttachments\"",
                "boolean",
                Boolean,
                FilterValue.Boolean),

            ["followUpStatus"] = new(
                "s.\"FollowUpStatus\"",
                "text",
                Set,
                FilterValue.Enum<SubmissionFollowUpStatus>()),
            ["formType"] = new(
                "s.\"FormType\"",
                "text",
                Set,
                FilterValue.Enum<FormType>()),
            ["questionsAnswered"] = new(
                "s.\"QuestionsAnswered\"",
                "text",
                Set,
                FilterValue.Enum<QuestionsAnsweredFilter>())
        };
}
