using Dapper;
using Feature.CitizenReports.ListFormsOverview;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports;

internal static class CitizenReportFilterQuery
{
    public static async Task<(List<AggregatedFormOverview> Entries, int TotalCount)> GetAggregatedFormsAsync(
        INpgsqlConnectionFactory connectionFactory,
        Guid electionRoundId,
        Guid ngoId,
        FilterRule? conditions,
        string? sortColumnName,
        bool isAscendingSorting,
        int pageNumber,
        int pageSize,
        CancellationToken ct)
    {
        var filter = new FilterSqlCompiler(V2ReportFilterFields.CitizenReports).Build(conditions);
        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", electionRoundId);
        parameters.Add("ngoId", ngoId);
        parameters.Add("offset", PaginationHelper.CalculateSkip(pageSize, pageNumber));
        parameters.Add("pageSize", pageSize);
        filter.AddTo(parameters);

        var countSql = $"""
                        SELECT COUNT(*)
                        FROM (
                            SELECT s."FormId"
                            FROM "GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                            WHERE {filter.Sql}
                            GROUP BY s."FormId"
                        ) filtered_forms;
                        """;

        var orderBy = GetOrderByClause(sortColumnName, isAscendingSorting);

        var listSql = $"""
                      WITH FILTERED_CITIZEN_REPORTS AS (
                          SELECT * 
                          FROM "GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                          WHERE {filter.Sql}
                      )
                      SELECT
                          "FormId",
                          "FormCode",
                          "FormName",
                          "FormDefaultLanguage",
                          COUNT("CitizenReportId") AS "NumberOfSubmissions",
                          COALESCE(SUM("NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                          COALESCE(SUM("NotesCount"), 0) AS "NumberOfNotes",
                          COALESCE(SUM("MediaFilesCount"), 0) AS "NumberOfMediaFiles"
                      FROM FILTERED_CITIZEN_REPORTS
                      GROUP BY "FormId", "FormCode", "FormName", "FormDefaultLanguage"
                      ORDER BY {orderBy}
                      OFFSET @offset ROWS
                      FETCH NEXT @pageSize ROWS ONLY;
                      """;

        using var connection = await connectionFactory.GetOpenConnectionAsync(ct);
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var rows = (await connection.QueryAsync<AggregatedFormOverview>(listSql, parameters)).ToList();
        return (rows, totalCount);
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
