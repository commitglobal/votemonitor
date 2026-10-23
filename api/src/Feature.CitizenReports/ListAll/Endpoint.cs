using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListAll;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<CitizenReportAdminRowModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports:all");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s => { s.Summary = "Lists all citizen reports in an election round (platform admins)"; });
    }

    public override async Task<PagedResponse<CitizenReportAdminRowModel>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = """
                  SELECT COUNT(*)
                  FROM "GetAllCitizenReports"(@electionRoundId, @level1, @level2, @level3, @level4, @level5,
                      @monitoringNgoId, @formId, @fromDate, @toDate, @searchText);

                  SELECT *
                  FROM "GetAllCitizenReports"(@electionRoundId, @level1, @level2, @level3, @level4, @level5,
                      @monitoringNgoId, @formId, @fromDate, @toDate, @searchText)
                  ORDER BY
                      CASE WHEN @sortExpression = 'TimeSubmitted ASC' THEN "TimeSubmitted" END ASC,
                      CASE WHEN @sortExpression = 'TimeSubmitted DESC' THEN "TimeSubmitted" END DESC,
                      CASE WHEN @sortExpression = 'FormCode ASC' THEN "FormCode" END ASC,
                      CASE WHEN @sortExpression = 'FormCode DESC' THEN "FormCode" END DESC,
                      CASE WHEN @sortExpression = 'NgoName ASC' THEN "NgoName" END ASC,
                      CASE WHEN @sortExpression = 'NgoName DESC' THEN "NgoName" END DESC,
                      CASE WHEN @sortExpression = 'Location ASC' THEN "Level1" END ASC,
                      CASE WHEN @sortExpression = 'Location DESC' THEN "Level1" END DESC,
                      "TimeSubmitted" DESC,
                      "CitizenReportId"
                  OFFSET @offset ROWS
                  FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var args = new
        {
            electionRoundId = req.ElectionRoundId,
            level1 = NullIfEmpty(req.Level1Filter),
            level2 = NullIfEmpty(req.Level2Filter),
            level3 = NullIfEmpty(req.Level3Filter),
            level4 = NullIfEmpty(req.Level4Filter),
            level5 = NullIfEmpty(req.Level5Filter),
            monitoringNgoId = req.MonitoringNgoId,
            formId = req.FormId,
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            searchText = string.IsNullOrWhiteSpace(req.SearchText) ? null : $"%{req.SearchText.Trim()}%",
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, args);

        var totalCount = multi.Read<int>().Single();
        var items = multi.Read<CitizenReportAdminRowModel>().ToList();

        return new PagedResponse<CitizenReportAdminRowModel>(items, totalCount, req.PageNumber, req.PageSize);
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private static string GetSortExpression(string? sortColumnName, bool isAscending)
    {
        var order = isAscending ? "ASC" : "DESC";
        var column = sortColumnName?.Trim().ToLowerInvariant() switch
        {
            "timesubmitted" => "TimeSubmitted",
            "formcode" => "FormCode",
            "ngoname" => "NgoName",
            "location" or "level1" => "Location",
            _ => null
        };

        return column is null ? "TimeSubmitted DESC" : $"{column} {order}";
    }
}
