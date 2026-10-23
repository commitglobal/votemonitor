using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.Queries;

namespace Feature.Review.StageCounts;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<StageCountModel[]>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/reviews/{recordType}:stage-counts");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Counts the records in each of the caller's NGO's review stages, using the report list's filter";
            s.Description = "Stages without reports are left out.";
        });
    }

    public override async Task<Results<Ok<StageCountModel[]>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(User, req.ElectionRoundId, req.RecordType))
        {
            return TypedResults.NotFound();
        }

        var (source, fields) = GetSource(req.RecordType);
        var filter = new FilterSqlCompiler(fields).Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("dataSource", req.DataSource.ToString());
        filter.AddTo(parameters);

        var sql = $"""
                   SELECT s."StageId", COUNT(1)::int AS "Count"
                   FROM {source} s
                   WHERE {filter.Sql}
                   GROUP BY s."StageId"
                   """;

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        var counts = await dbConnection.QueryAsync<StageCountModel>(sql, parameters);

        return TypedResults.Ok(counts.ToArray());
    }

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
}
