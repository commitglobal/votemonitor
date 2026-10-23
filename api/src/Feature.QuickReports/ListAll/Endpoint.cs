using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.QuickReports.ListAll;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<QuickReportAdminRowModel>>
{
    private const string Source = """
        "GetPlatformAdminQuickReports"(@electionRoundId, @level1, @level2, @level3, @level4, @level5, @monitoringNgoId,
            @fromDate, @toDate, @searchText) QR
        """;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports:all");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Lists all quick reports in an election round with anonymized observer data (platform admins)";
        });
    }

    public override async Task<PagedResponse<QuickReportAdminRowModel>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = $"""
                   SELECT COUNT(*)
                   FROM {Source};

                   SELECT QR.*
                   FROM {Source}
                   ORDER BY
                       CASE WHEN @sortExpression = 'Timestamp ASC' THEN QR."Timestamp" END ASC,
                       CASE WHEN @sortExpression = 'Timestamp DESC' THEN QR."Timestamp" END DESC,
                       CASE WHEN @sortExpression = 'Title ASC' THEN QR."Title" END ASC,
                       CASE WHEN @sortExpression = 'Title DESC' THEN QR."Title" END DESC,
                       CASE WHEN @sortExpression = 'Number ASC' THEN QR."Number" END ASC,
                       CASE WHEN @sortExpression = 'Number DESC' THEN QR."Number" END DESC,
                       CASE WHEN @sortExpression = 'NgoName ASC' THEN QR."NgoName" END ASC,
                       CASE WHEN @sortExpression = 'NgoName DESC' THEN QR."NgoName" END DESC,
                       CASE WHEN @sortExpression = 'Location ASC' THEN QR."Level1" END ASC,
                       CASE WHEN @sortExpression = 'Location DESC' THEN QR."Level1" END DESC,
                       QR."Timestamp" DESC,
                       QR."Id"
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
        var items = multi.Read<QuickReportAdminRowModel>().Select(x => x.WithMaskedObserverData()).ToList();

        return new PagedResponse<QuickReportAdminRowModel>(items, totalCount, req.PageNumber, req.PageSize);
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private static string GetSortExpression(string? sortColumnName, bool isAscending)
    {
        var order = isAscending ? "ASC" : "DESC";
        var column = sortColumnName?.Trim().ToLowerInvariant() switch
        {
            "timestamp" => "Timestamp",
            "title" => "Title",
            "number" => "Number",
            "ngoname" => "NgoName",
            "location" or "level1" => "Location",
            _ => null
        };

        return column is null ? "Timestamp DESC" : $"{column} {order}";
    }
}
