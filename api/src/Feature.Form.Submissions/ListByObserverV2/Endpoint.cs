using Feature.Form.Submissions.ListByObserver;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListByObserverV2;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<ObserverSubmissionOverview>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions:byObserverV2");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminOrStaff);

        Summary(x => { x.Summary = "Form submissions aggregated by observer (v2)"; });
    }

    public override async Task<Results<Ok<PagedResponse<ObserverSubmissionOverview>>, NotFound>> ExecuteAsync(
        Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var compiler = new FilterSqlCompiler(SubmissionFilterFields.All);
        var filter = compiler.Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("dataSource", req.DataSource.ToString());
        parameters.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        parameters.Add("pageSize", req.PageSize);

        filter.AddTo(parameters);

        var orderBy = GetOrderByClause(req.SortColumnName, req.IsAscendingSorting);

        // Without a filter every observer is listed (observers with no submissions get zeros).
        // With a filter only observers that have at least one matching submission are listed.
        var countSql = req.Filter is null
            ? """
              SELECT COUNT(*)
              FROM "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
              """
            : $"""
               SELECT COUNT(DISTINCT s."MonitoringObserverId")
               FROM "GetFormSubmissionEntries"(@electionRoundId, @ngoId, @dataSource) s
               WHERE {filter.Sql}
               """;

        var observersJoin = req.Filter is null ? "LEFT JOIN" : "INNER JOIN";

        var selectSql = $"""
                         WITH
                           SUBMISSIONS AS (
                             SELECT
                               s."MonitoringObserverId",
                               SUM(s."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                               COUNT(DISTINCT s."PollingStationId") AS "NumberOfLocations",
                               COUNT(*) AS "NumberOfFormsSubmitted",
                               BOOL_OR(s."FollowUpStatus" = 'NeedsFollowUp') AS "NeedsFollowUp"
                             FROM "GetFormSubmissionEntries"(@electionRoundId, @ngoId, @dataSource) s
                             WHERE {filter.Sql}
                             GROUP BY s."MonitoringObserverId"
                           )
                         SELECT
                           MO."MonitoringObserverId",
                           MO."DisplayName" AS "ObserverName",
                           MO."PhoneNumber",
                           MO."Email",
                           MO."Tags",
                           MO."NgoName",
                           MO."IsOwnObserver",
                           COALESCE(S."NumberOfFlaggedAnswers", 0) AS "NumberOfFlaggedAnswers",
                           COALESCE(S."NumberOfLocations", 0) AS "NumberOfLocations",
                           COALESCE(S."NumberOfFormsSubmitted", 0) AS "NumberOfFormsSubmitted",
                           CASE WHEN S."NeedsFollowUp" THEN 'NeedsFollowUp' END AS "FollowUpStatus"
                         FROM "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                         {observersJoin} SUBMISSIONS S ON S."MonitoringObserverId" = MO."MonitoringObserverId"
                         ORDER BY {orderBy}, MO."MonitoringObserverId"
                         LIMIT @pageSize OFFSET @offset
                         """;

        var sql = $"{countSql};\n{selectSql}";

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);

        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<ObserverSubmissionOverview>().ToList();

        return TypedResults.Ok(
            new PagedResponse<ObserverSubmissionOverview>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"""ObserverName"" ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""ObserverName"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""Email"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""PhoneNumber"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""Tags"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfLocations),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfLocations"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfFormsSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfFormsSubmitted"" {sortOrder}";
        }

        return @"""ObserverName"" ASC";
    }
}
