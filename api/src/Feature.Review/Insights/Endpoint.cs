using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.Queries;

namespace Feature.Review.Insights;

/// <summary>
/// Custom insights: a record type's list (with its filter) measured by one axis and optionally split by a second, for
/// a bar chart and its table.
/// </summary>
public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<InsightsModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/reviews/{recordType}:insights");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Measures the caller's NGO's records of one type by an axis, optionally grouped by another";
            s.Description =
                "Uses the report list's rows and filter. Bars come largest first (day and hour axes in time order), " +
                "the records without a value (no reviewer, no place at that level…) under a null key. A record with " +
                "several reviewers, tags or labels counts once under each, but once in every total.";
        });
    }

    public override async Task<Results<Ok<InsightsModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(User, req.ElectionRoundId, req.RecordType))
        {
            return TypedResults.NotFound();
        }

        var axes = InsightAxes.For(req.RecordType);
        var x = axes[req.XAxis];
        var g = req.GroupBy is null ? null : axes[req.GroupBy];
        var metric = InsightMetrics.For(req.RecordType)[req.Metric];
        var (source, fields) = GetSource(req.RecordType);
        var filter = new FilterSqlCompiler(fields).Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("dataSource", req.DataSource.ToString());
        parameters.Add("timeZone", req.TimeZone);
        filter.AddTo(parameters);

        string Query(params (InsightAxis Axis, string Alias)[] by)
        {
            var select = by.Select(b => $"{b.Axis.Key(b.Alias)} AS \"{b.Alias}Key\", MIN({b.Axis.Label(b.Alias)}) AS \"{b.Alias}Label\", ");
            var joins = by.Select(b => b.Axis.JoinSql(b.Alias));
            var groupBy = by.Length == 0 ? "" : "GROUP BY " + string.Join(", ", by.Select(b => b.Axis.Key(b.Alias)));
            return $"""
                    SELECT {string.Concat(select)}({metric})::bigint AS "Value"
                    FROM {source} s
                    {string.Join("\n", joins)}
                    WHERE {filter.Sql}
                    {groupBy}
                    """;
        }

        var statements = new List<string> { Query(), Query((x, "x")) };
        if (g is not null)
        {
            statements.Add(Query((g, "g")));
            statements.Add(Query((x, "x"), (g, "g")));
        }

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(
            new CommandDefinition(string.Join(";\n", statements), parameters, cancellationToken: ct));

        var total = (await multi.ReadAsync<long>()).Single();
        var bars = (await multi.ReadAsync<Row>()).ToList();
        var groups = g is null ? null : (await multi.ReadAsync<Row>()).ToList();
        var cells = g is null ? null : (await multi.ReadAsync<Row>()).ToLookup(c => c.XKey);

        var model = new InsightsModel(
            req.XAxis,
            req.GroupBy,
            req.Metric,
            total,
            Order(bars, x.IsTime, b => b.XKey, b => b.Value)
                .Select(b => new InsightBarModel(b.XKey, b.XLabel, b.Value,
                    cells is null
                        ? null
                        : Order(cells[b.XKey], g!.IsTime, c => c.GKey, c => c.Value)
                            .Select(c => new InsightBucketModel(c.GKey, c.GLabel, c.Value))
                            .ToArray()))
                .ToArray(),
            groups is null
                ? null
                : Order(groups, g!.IsTime, r => r.GKey, r => r.Value)
                    .Select(r => new InsightBucketModel(r.GKey, r.GLabel, r.Value))
                    .ToArray());

        return TypedResults.Ok(model);
    }

    /// <summary>Time axes in time order; the rest largest first, then by key; the records without a value last.</summary>
    private static IEnumerable<T> Order<T>(IEnumerable<T> rows, bool isTime, Func<T, string?> key, Func<T, long> value) =>
        isTime
            ? rows.OrderBy(r => key(r) is null).ThenBy(key, StringComparer.Ordinal)
            : rows.OrderBy(r => key(r) is null).ThenByDescending(value).ThenBy(key, StringComparer.Ordinal);

    private static (string Source, IReadOnlyDictionary<string, FilterField> Fields) GetSource(ReportRecordType recordType)
    {
        if (recordType == ReportRecordType.FormSubmission)
        {
            return ("\"GetFormSubmissionEntries\"(@electionRoundId, @ngoId, @dataSource)", V2ReportFilterFields.FormSubmissions);
        }

        if (recordType == ReportRecordType.QuickReport)
        {
            return ("\"GetQuickReportEntries\"(@electionRoundId, @ngoId, @dataSource)", V2ReportFilterFields.QuickReports);
        }

        if (recordType == ReportRecordType.IncidentReport)
        {
            return ("\"GetIncidentReportEntries\"(@electionRoundId, @ngoId, @dataSource)", V2ReportFilterFields.IncidentReports);
        }

        return ("\"GetCitizenReportEntries\"(@electionRoundId, @ngoId)", V2ReportFilterFields.CitizenReports);
    }

    private sealed class Row
    {
        public string? XKey { get; init; }
        public string? XLabel { get; init; }
        public string? GKey { get; init; }
        public string? GLabel { get; init; }
        public long Value { get; init; }
    }
}
