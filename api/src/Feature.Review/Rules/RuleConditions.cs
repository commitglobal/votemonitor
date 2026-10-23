using System.Text.Json;
using Feature.Review.Services;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Queries;

namespace Feature.Review.Rules;

/// <summary>
/// An automation rule's conditions: the advanced filter of the V2 lists, limited to the fields a kind of rule can use
/// for a record type, checked against the record's row in the record type's <c>Get*Entries</c> function as the
/// rule's NGO sees it.
/// </summary>
public static class RuleConditions
{
    // The browser's limits for a query.
    private const int MaxRules = 30;
    private const int MaxDepth = 3;
    private const int MaxValues = 100;
    private const int MaxTextLength = 256;

    private static readonly string[] Levels = ["level1", "level2", "level3", "level4", "level5"];

    private static readonly Dictionary<(AutomationRuleKind, ReportRecordType), IReadOnlyDictionary<string, FilterField>>
        FieldsByKindAndRecordType = AutomationRuleKind.List
            .SelectMany(kind => ReportRecordType.List.Select(recordType => (kind, recordType)))
            .ToDictionary(x => x, x => BuildFields(x.kind, x.recordType));

    /// <summary>The fields a kind of rule can use for a record type, by their filter names.</summary>
    private static IReadOnlyDictionary<string, FilterField> Fields(AutomationRuleKind kind, ReportRecordType recordType) =>
        FieldsByKindAndRecordType[(kind, recordType)];

    /// <summary>Whether a kind of rule can use answer conditions for a record type: severity rules, on records with answers.</summary>
    private static bool AllowsAnswers(AutomationRuleKind kind, ReportRecordType recordType) =>
        kind == AutomationRuleKind.Severity && recordType != ReportRecordType.QuickReport;

    /// <summary>
    /// Why the conditions can't be saved on a rule of this kind for these record types, or null when they can: every
    /// field and answer condition must be allowed for every record type, within the browser's limits.
    /// </summary>
    public static string? FindError(FilterRule? conditions,
        AutomationRuleKind kind,
        IReadOnlyCollection<ReportRecordType> recordTypes)
    {
        if (conditions is null)
        {
            return null;
        }

        var leaves = new List<FilterRule>();
        if (Depth(conditions, leaves) > MaxDepth)
        {
            return $"Conditions can be nested at most {MaxDepth} levels deep";
        }

        if (leaves.Count > MaxRules)
        {
            return $"Conditions can have at most {MaxRules} rules";
        }

        foreach (var leaf in leaves)
        {
            var error = FindLeafError(leaf, kind, recordTypes);
            if (error is not null)
            {
                return error;
            }
        }

        foreach (var recordType in recordTypes)
        {
            try
            {
                Compile(conditions, kind, recordType, "");
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }
        }

        return null;
    }

    /// <summary>
    /// The ids of the rules whose conditions the record matches, as the report's NGO sees it. A condition on a form or
    /// question that no longer exists doesn't match.
    /// </summary>
    public static async Task<HashSet<Guid>> MatchAsync(this VoteMonitorContext context,
        ReviewContext report,
        IReadOnlyList<AutomationRule> rules,
        CancellationToken ct)
    {
        if (rules.Count == 0)
        {
            return [];
        }

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", report.Target.ElectionRoundId);
        parameters.Add("ngoId", report.NgoId);
        parameters.Add("recordId", report.Target.RecordId);

        // One CASE per rule, so the record's row is read once: the indexes of the rules it matches.
        var cases = rules.Select((rule, index) =>
        {
            var filter = Compile(FilterRuleJson.Deserialize(rule.Conditions), rule.Kind, report.Target.RecordType,
                $"r{index}_");
            filter.AddTo(parameters);
            return $"CASE WHEN {filter.Sql} THEN {index} END";
        });

        var sql = $"""
                   SELECT array_remove(ARRAY[{string.Join(", ", cases)}]::int[], NULL)
                   FROM {EntrySource(report.Target.RecordType)} s
                   """;

        var matches = await context.Database.GetDbConnection()
            .QueryFirstOrDefaultAsync<int[]>(new CommandDefinition(sql, parameters, cancellationToken: ct));

        return (matches ?? []).Select(index => rules[index].Id).ToHashSet();
    }

    private static CompiledFilter Compile(FilterRule? conditions,
        AutomationRuleKind kind,
        ReportRecordType recordType,
        string parameterPrefix)
    {
        var compiler = new FilterSqlCompiler(Fields(kind, recordType),
            parameterPrefixer: name => $"@{parameterPrefix}{name}");
        var filter = compiler.Build(conditions);

        var parameters = new DynamicParameters();
        foreach (var name in filter.Parameters.ParameterNames)
        {
            parameters.Add(parameterPrefix + name, filter.Parameters.Get<object?>(name));
        }

        return filter with { Parameters = parameters };
    }

    private static string? FindLeafError(FilterRule rule,
        AutomationRuleKind kind,
        IReadOnlyCollection<ReportRecordType> recordTypes)
    {
        switch (rule)
        {
            case FieldRule field:
                if (!ReportRecordType.List.Any(x => Fields(kind, x).ContainsKey(field.Field)))
                {
                    return $"Field '{field.Field}' can't be used in {KindLabel(kind)}";
                }

                var notAllowed = recordTypes.FirstOrDefault(x => !Fields(kind, x).ContainsKey(field.Field));
                if (notAllowed is not null)
                {
                    return $"Field '{field.Field}' can't be used for {RecordTypeLabel(notAllowed)}";
                }

                return FindValueError(field.Value, field.Values, field.From, field.To);

            case AnswerRule answer:
                if (kind != AutomationRuleKind.Severity)
                {
                    return $"Answer conditions can't be used in {KindLabel(kind)}";
                }

                var withoutAnswers = recordTypes.FirstOrDefault(x => !AllowsAnswers(kind, x));
                if (withoutAnswers is not null)
                {
                    return $"Answer conditions can't be used for {RecordTypeLabel(withoutAnswers)}";
                }

                return FindValueError(answer.Value, answer.Values, answer.From, answer.To);

            default:
                return null;
        }
    }

    private static string? FindValueError(JsonElement? value,
        List<JsonElement>? values,
        JsonElement? from,
        JsonElement? to)
    {
        if (values?.Count > MaxValues)
        {
            return $"A condition can have at most {MaxValues} values";
        }

        IEnumerable<JsonElement?> all = [value, from, to, ..values?.Select(x => (JsonElement?)x) ?? []];
        return all.Any(x => x?.ValueKind == JsonValueKind.String && x.Value.GetString()!.Length > MaxTextLength)
            ? $"Condition values can be at most {MaxTextLength} characters"
            : null;
    }

    /// <summary>How many and / or groups deep the rule is ("not" adds no level); collects the field and answer rules.</summary>
    private static int Depth(FilterRule rule, List<FilterRule> leaves)
    {
        if (rule is not LogicalFilter logical)
        {
            leaves.Add(rule);
            return 0;
        }

        var childDepth = (logical.Values ?? []).Select(x => Depth(x, leaves)).DefaultIfEmpty(0).Max();
        return rule is NotFilter ? childDepth : childDepth + 1;
    }

    /// <summary>The record's row in its record type's entries function, which only reads that record's base row.</summary>
    private static string EntrySource(ReportRecordType recordType)
    {
        if (recordType == ReportRecordType.FormSubmission)
        {
            return "\"GetFormSubmissionEntries\"(@electionRoundId, @ngoId, 'Coalition', @recordId)";
        }

        if (recordType == ReportRecordType.QuickReport)
        {
            return "\"GetQuickReportEntries\"(@electionRoundId, @ngoId, 'Coalition', @recordId)";
        }

        if (recordType == ReportRecordType.IncidentReport)
        {
            return "\"GetIncidentReportEntries\"(@electionRoundId, @ngoId, 'Coalition', @recordId)";
        }

        return "\"GetCitizenReportEntries\"(@electionRoundId, @ngoId, @recordId)";
    }

    private static IReadOnlyDictionary<string, FilterField> BuildFields(AutomationRuleKind kind,
        ReportRecordType recordType)
    {
        var listFields = recordType == ReportRecordType.FormSubmission ? V2ReportFilterFields.FormSubmissions
            : recordType == ReportRecordType.QuickReport ? V2ReportFilterFields.QuickReports
            : recordType == ReportRecordType.IncidentReport ? V2ReportFilterFields.IncidentReports
            : V2ReportFilterFields.CitizenReports;

        var fields = Levels.ToDictionary(x => x, x => listFields[x]);
        if (recordType != ReportRecordType.QuickReport)
        {
            fields["formId"] = listFields["formId"];
        }

        if (recordType == ReportRecordType.FormSubmission || recordType == ReportRecordType.IncidentReport)
        {
            fields["formType"] = listFields["formType"];
        }

        if (kind == AutomationRuleKind.Reviewer && recordType != ReportRecordType.CitizenReport)
        {
            // The quick report list doesn't filter by tags, but its entries carry them like the submissions'.
            fields["tags"] = V2ReportFilterFields.FormSubmissions["tags"];
        }

        return fields;
    }

    private static string KindLabel(AutomationRuleKind kind) =>
        kind == AutomationRuleKind.Reviewer ? "reviewer rules" : "severity rules";

    private static string RecordTypeLabel(ReportRecordType recordType) =>
        recordType == ReportRecordType.FormSubmission ? "form submissions"
        : recordType == ReportRecordType.QuickReport ? "quick reports"
        : recordType == ReportRecordType.IncidentReport ? "incident reports"
        : "citizen reports";
}
