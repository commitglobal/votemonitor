using Dapper;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Queries;

namespace Feature.Notifications.RecipientsV2;

/// <summary>Which observers a notification goes to, as notifications:listRecipientsV2 and :sendV2 take it.</summary>
public interface IRecipientsV2Filters
{
    Guid ElectionRoundId { get; }
    Guid NgoId { get; }

    /// <summary>
    /// A v2 filter over the observer and its records; no filter: every observer of the NGO. Observer fields:
    /// monitoringObserverId, name, email, phoneNumber (contains ignores case), tags, monitoringObserverStatus,
    /// hasFormSubmissions, hasQuickReports, hasIncidentReports. Record fields carry the
    /// kind of record as a prefix and are the fields of that report's list: fs: (form submissions, e.g. fs:level1,
    /// fs:formId), qr: (quick reports, e.g. qr:incidentCategory), ir: (incident reports, e.g. ir:locationType).
    /// A record rule matches observers with such a record; record rules of one kind in the same "and" must match the
    /// same record, and a "not" keeps the observers without one.
    /// </summary>
    FilterRule? Filter { get; }
}

public static class RecipientsV2Query
{
    private sealed record RecordSource(string Cte, string Function, IReadOnlyDictionary<string, FilterField> Fields);

    private static readonly IReadOnlyDictionary<string, RecordSource> Sources =
        new Dictionary<string, RecordSource>
        {
            ["fs"] = new("FormSubmissionEntries", "GetFormSubmissionEntries", V2ReportFilterFields.FormSubmissions),
            ["qr"] = new("QuickReportEntries", "GetQuickReportEntries", V2ReportFilterFields.QuickReports),
            ["ir"] = new("IncidentReportEntries", "GetIncidentReportEntries", V2ReportFilterFields.IncidentReports)
        };

    private static string HasRecords(string cte) =>
        $"""(o."Id" IN (SELECT r."MonitoringObserverId" FROM "{cte}" r))""";

    /// <summary>The fields about the observer itself (alias o, a row of "Observers").</summary>
    public static readonly IReadOnlyDictionary<string, FilterField> ObserverFields =
        new Dictionary<string, FilterField>
        {
            ["monitoringObserverId"] = new("o.\"Id\"", "uuid", FilterOperators.Set, FilterValue.Guid),
            ["name"] = new("o.\"DisplayName\"", "text", FilterOperators.Text, FilterValue.String, IgnoreCase: true),
            ["email"] = new("o.\"Email\"::TEXT", "text", FilterOperators.Text, FilterValue.String, IgnoreCase: true),
            ["phoneNumber"] = new("o.\"PhoneNumber\"", "text", FilterOperators.Text, FilterValue.String,
                IgnoreCase: true),
            ["tags"] = new("COALESCE(o.\"Tags\", '{}')", "text", FilterOperators.Array, FilterValue.String,
                IsArray: true),
            ["monitoringObserverStatus"] = new("o.\"Status\"", "text", FilterOperators.Set,
                FilterValue.Enum<MonitoringObserverStatus>()),
            ["hasFormSubmissions"] = new(HasRecords(Sources["fs"].Cte), "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["hasQuickReports"] = new(HasRecords(Sources["qr"].Cte), "boolean", FilterOperators.Boolean,
                FilterValue.Boolean),
            ["hasIncidentReports"] = new(HasRecords(Sources["ir"].Cte), "boolean", FilterOperators.Boolean,
                FilterValue.Boolean)
        };

    /// <summary>Why the filter can't be compiled; null when it can.</summary>
    public static string? Validate(IRecipientsV2Filters filters)
    {
        if (filters.Filter is null)
        {
            return null;
        }

        try
        {
            new RecipientsFilterCompiler(new DynamicParameters()).Compile(filters.Filter);
            return null;
        }
        catch (ArgumentException e)
        {
            return e.Message;
        }
    }

    /// <summary>
    /// The <c>WITH</c> clause defining "Observers" (the NGO's monitoring observers: Id, ObserverId, Tags, Status,
    /// DisplayName, Email, PhoneNumber) and "Recipients" (MonitoringObserverId, the ones matching the filter), adding
    /// its parameters.
    /// </summary>
    public static string With(IRecipientsV2Filters filters, DynamicParameters parameters)
    {
        var filter = filters.Filter is null ? "TRUE" : new RecipientsFilterCompiler(parameters).Compile(filters.Filter);

        parameters.Add("electionRoundId", filters.ElectionRoundId);
        parameters.Add("ngoId", filters.NgoId);

        // The records of each kind, read once; Postgres skips the ones the filter doesn't use.
        var records = string.Join(",\n", Sources.Values.Select(source =>
            $"""
             "{source.Cte}" AS MATERIALIZED (
                 SELECT * FROM "{source.Function}"(@electionRoundId, @ngoId, 'Ngo')
             )
             """));

        return $"""
                WITH {records},
                "Observers" AS (
                    SELECT MO."Id", MO."ObserverId", MO."Tags", MO."Status", U."DisplayName", U."Email", U."PhoneNumber"
                    FROM "MonitoringObservers" MO
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                    WHERE MN."ElectionRoundId" = @electionRoundId
                      AND MN."NgoId" = @ngoId
                ),
                "Recipients" AS (
                    SELECT o."Id" AS "MonitoringObserverId"
                    FROM "Observers" o
                    WHERE {filter}
                )
                """;
    }

    /// <summary>
    /// Compiles the filter against an observer (alias o): observer fields directly, record rules as "the observer has
    /// such a record". Each piece is compiled with its own parameter prefix, so their parameters never collide.
    /// </summary>
    private sealed class RecipientsFilterCompiler(DynamicParameters parameters)
    {
        private int _index;

        public string Compile(FilterRule rule) =>
            rule switch
            {
                AndFilter and => CompileGroup(and.Values, "AND"),
                OrFilter or => CompileGroup(or.Values, "OR"),
                NotFilter not => not.Values is { Count: 1 }
                    ? $"(NOT {Compile(not.Values[0])})"
                    : throw new ArgumentException("'not' must contain exactly one rule."),
                FieldRule field => RecordKind(field) is { } kind
                    ? CompileRecords(kind, [field])
                    : CompileWith(ObserverFields, field),
                _ => throw new ArgumentException($"Unsupported filter rule: {rule.GetType().Name}.")
            };

        private string CompileGroup(List<FilterRule>? rules, string joiner)
        {
            if (rules is null)
                throw new ArgumentException("Logical rule must contain a values array.");

            if (rules.Count == 0)
                return joiner == "AND" ? "TRUE" : "FALSE";

            if (joiner == "OR")
                return "(" + string.Join(" OR ", rules.Select(Compile)) + ")";

            // Record rules of one kind in the same "and" are about the same record.
            var recordRules = rules
                .OfType<FieldRule>()
                .Where(x => RecordKind(x) is not null)
                .GroupBy(x => RecordKind(x)!)
                .Select(group => CompileRecords(group.Key, group.ToList()));
            var otherRules = rules
                .Where(x => x is not FieldRule field || RecordKind(field) is null)
                .Select(Compile);

            return "(" + string.Join(" AND ", recordRules.Concat(otherRules)) + ")";
        }

        private string CompileRecords(string kind, IReadOnlyList<FieldRule> rules)
        {
            var source = Sources[kind];
            var unprefixed = rules
                .Select(FilterRule (x) => x with { Field = x.Field[(kind.Length + 1)..] })
                .ToList();
            var condition = CompileWith(source.Fields, new AndFilter { Values = unprefixed });

            return $"""(o."Id" IN (SELECT s."MonitoringObserverId" FROM "{source.Cte}" s WHERE {condition}))""";
        }

        private string CompileWith(IReadOnlyDictionary<string, FilterField> fields, FilterRule rule)
        {
            var prefix = $"f{_index++}_";
            var compiled = new FilterSqlCompiler(fields, parameterPrefixer: name => $"@{prefix}{name}").Build(rule);
            compiled.AddTo(parameters, prefix);
            return compiled.Sql;
        }

        /// <summary>fs, qr or ir for a record field (fs:level1); null for an observer field.</summary>
        private static string? RecordKind(FieldRule rule)
        {
            var separator = rule.Field.IndexOf(':');
            if (separator <= 0)
            {
                return null;
            }

            var kind = rule.Field[..separator];
            return Sources.ContainsKey(kind) ? kind : null;
        }
    }
}
