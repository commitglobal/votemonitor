using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.QuickReports.ListAll;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<QuickReportAdminRowModel>>
{
    private const string FromWhere = """
        FROM "QuickReports" QR
            LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
            INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
            INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
            INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
            LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
        WHERE QR."ElectionRoundId" = @electionRoundId
            AND (@level1 IS NULL OR PS."Level1" = @level1)
            AND (@level2 IS NULL OR PS."Level2" = @level2)
            AND (@level3 IS NULL OR PS."Level3" = @level3)
            AND (@level4 IS NULL OR PS."Level4" = @level4)
            AND (@level5 IS NULL OR PS."Level5" = @level5)
            AND (@monitoringNgoId IS NULL OR MN."Id" = @monitoringNgoId)
            AND (@fromDate IS NULL OR QR."LastUpdatedAt" >= @fromDate::timestamp)
            AND (@toDate IS NULL OR QR."LastUpdatedAt" <= @toDate::timestamp)
            AND (@searchText IS NULL OR QR."Title" ILIKE @searchText OR PS."Number" ILIKE @searchText
                 OR PS."Address" ILIKE @searchText OR QR."PollingStationDetails" ILIKE @searchText)
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
                   {FromWhere};

                   SELECT
                       QR."Id",
                       QR."LastUpdatedAt" AS "Timestamp",
                       QR."Title",
                       QR."IncidentCategory",
                       QR."QuickReportLocationType",
                       QR."PollingStationDetails",
                       PS."Id" AS "PollingStationId",
                       PS."Level1",
                       PS."Level2",
                       PS."Level3",
                       PS."Level4",
                       PS."Level5",
                       PS."Number",
                       PS."Address",
                       MN."Id" AS "MonitoringNgoId",
                       N."Name" AS "NgoName",
                       MO."Id" AS "MonitoringObserverId",
                       U."DisplayName" AS "ObserverName",
                       U."Email" AS "ObserverEmail",
                       U."PhoneNumber" AS "ObserverPhoneNumber",
                       QR."FollowUpStatus"
                   {FromWhere}
                   ORDER BY
                       CASE WHEN @sortExpression = 'Timestamp ASC' THEN QR."LastUpdatedAt" END ASC,
                       CASE WHEN @sortExpression = 'Timestamp DESC' THEN QR."LastUpdatedAt" END DESC,
                       CASE WHEN @sortExpression = 'Title ASC' THEN QR."Title" END ASC,
                       CASE WHEN @sortExpression = 'Title DESC' THEN QR."Title" END DESC,
                       CASE WHEN @sortExpression = 'Number ASC' THEN PS."Number" END ASC,
                       CASE WHEN @sortExpression = 'Number DESC' THEN PS."Number" END DESC,
                       CASE WHEN @sortExpression = 'NgoName ASC' THEN N."Name" END ASC,
                       CASE WHEN @sortExpression = 'NgoName DESC' THEN N."Name" END DESC,
                       CASE WHEN @sortExpression = 'Location ASC' THEN PS."Level1" END ASC,
                       CASE WHEN @sortExpression = 'Location DESC' THEN PS."Level1" END DESC,
                       QR."LastUpdatedAt" DESC,
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
