using Dapper;
using Feature.CitizenReports.ListFormsOverview;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListFormsOverviewV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<AggregatedFormOverview>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports:byFormV2");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);

        Summary(x => { x.Summary = "Citizen report submissions aggregated by form (v2)"; });
    }

    public override async Task<Results<Ok<PagedResponse<AggregatedFormOverview>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var compiler = new FilterSqlCompiler(V2ReportFilterFields.CitizenReports);
        var filter = compiler.Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        parameters.Add("pageSize", req.PageSize);

        filter.AddTo(parameters);

        var orderBy = GetOrderByClause(req.SortColumnName, req.IsAscendingSorting);

        var countSql = $"""
                        SELECT COUNT(DISTINCT s."FormId")
                        FROM "GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                        WHERE {filter.Sql}
                        """;

        var selectSql = $"""
                         SELECT s."FormId",
                                s."FormCode",
                                s."FormName",
                                s."FormDefaultLanguage",
                                COUNT(*) AS "NumberOfSubmissions",
                                COALESCE(SUM(s."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                                COALESCE(SUM(s."NotesCount"), 0) AS "NumberOfNotes",
                                COALESCE(SUM(s."MediaFilesCount"), 0) AS "NumberOfMediaFiles"
                         FROM "GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                         WHERE {filter.Sql}
                         GROUP BY s."FormId", s."FormCode", s."FormName", s."FormDefaultLanguage"
                         ORDER BY {orderBy}, s."FormId"
                         LIMIT @pageSize OFFSET @offset
                         """;

        var sql = $"{countSql};\n{selectSql}";

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);

        var totalRowCount = multi.Read<int>().Single();
        var aggregatedFormOverviews = multi.Read<AggregatedFormOverview>().ToList();

        return TypedResults.Ok(
            new PagedResponse<AggregatedFormOverview>(aggregatedFormOverviews, totalRowCount, req.PageNumber,
                req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"""FormCode"" ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""FormCode"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfSubmissions),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfSubmissions"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfNotes),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfNotes"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfMediaFiles),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfMediaFiles"" {sortOrder}";
        }

        return @"""FormCode"" ASC";
    }
}
