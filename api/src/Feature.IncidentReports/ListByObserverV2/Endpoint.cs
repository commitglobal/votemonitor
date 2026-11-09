using Dapper;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.IncidentReports.ListByObserverV2;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<ObserverIncidentReportsOverviewV2>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports:byObserverV2");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);

        Summary(x => { x.Summary = "Incident reports aggregated by observer (v2)"; });
    }

    public override async Task<Results<Ok<PagedResponse<ObserverIncidentReportsOverviewV2>>, NotFound>> ExecuteAsync(
        Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var compiler = new FilterSqlCompiler(V2ReportFilterFields.IncidentReports);
        var filter = compiler.Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("dataSource", req.DataSource.ToString());
        parameters.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        parameters.Add("pageSize", req.PageSize);

        filter.AddTo(parameters);

        var orderBy = GetOrderByClause(req.SortColumnName, req.IsAscendingSorting);

        // Without a filter every observer is listed (observers with no incident reports get zeros).
        // With a filter only observers that have at least one matching incident report are listed.
        var countSql = req.Filter is null
            ? """
              SELECT COUNT(*)
              FROM "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
              """
            : $"""
               SELECT COUNT(DISTINCT s."MonitoringObserverId")
               FROM "GetIncidentReportEntries"(@electionRoundId, @ngoId, @dataSource) s
               WHERE {filter.Sql}
               """;

        var observersJoin = req.Filter is null ? "LEFT JOIN" : "INNER JOIN";

        var selectSql = $"""
                         WITH
                           INCIDENT_REPORTS AS (
                             SELECT
                               s."MonitoringObserverId",
                               SUM(s."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                               COUNT(*) AS "NumberOfIncidentsSubmitted"
                             FROM "GetIncidentReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                             WHERE {filter.Sql}
                             GROUP BY s."MonitoringObserverId"
                           ),
                           -- Per observer, how many of the reports are on each of the caller NGO's follow-up statuses.
                           FOLLOW_UPS AS (
                             SELECT s."MonitoringObserverId", FUS."Id", FUS."Name", FUS."Color", FUS."Icon", FUS."Sequence", COUNT(*) AS "Count"
                             FROM "GetIncidentReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                             JOIN "FollowUpStatuses" FUS ON FUS."Id" = {V2ReportFilterFields.FollowUpStatusIdSql("s.\"Id\"", "IncidentReport")}
                             WHERE {filter.Sql}
                             GROUP BY s."MonitoringObserverId", FUS."Id", FUS."Name", FUS."Color", FUS."Icon", FUS."Sequence"
                           )
                         SELECT
                           MO."MonitoringObserverId",
                           MO."DisplayName" AS "ObserverName",
                           MO."PhoneNumber",
                           MO."Email",
                           MO."Tags",
                           COALESCE(IR."NumberOfFlaggedAnswers", 0) AS "NumberOfFlaggedAnswers",
                           COALESCE(IR."NumberOfIncidentsSubmitted", 0) AS "NumberOfIncidentsSubmitted",
                           (SELECT JSON_AGG(JSON_BUILD_OBJECT('id', F."Id", 'name', F."Name", 'color', F."Color", 'icon', F."Icon", 'count', F."Count") ORDER BY F."Sequence")
                            FROM FOLLOW_UPS F WHERE F."MonitoringObserverId" = MO."MonitoringObserverId")::TEXT AS "FollowUpsJson"
                         FROM "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                         {observersJoin} INCIDENT_REPORTS IR ON IR."MonitoringObserverId" = MO."MonitoringObserverId"
                         ORDER BY {orderBy}, MO."MonitoringObserverId"
                         LIMIT @pageSize OFFSET @offset
                         """;

        var sql = $"{countSql};\n{selectSql}";

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);

        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<ObserverIncidentReportsRow>().Select(x => x.ToModel()).ToList();

        return TypedResults.Ok(
            new PagedResponse<ObserverIncidentReportsOverviewV2>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"""ObserverName"" ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverviewV2.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""ObserverName"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverviewV2.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""Email"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverviewV2.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""PhoneNumber"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverviewV2.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""Tags"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverviewV2.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverviewV2.NumberOfIncidentsSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfIncidentsSubmitted"" {sortOrder}";
        }

        return @"""ObserverName"" ASC";
    }
}
