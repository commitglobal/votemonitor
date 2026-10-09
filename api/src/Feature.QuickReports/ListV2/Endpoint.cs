using Authorization.Policies;
using Dapper;
using Feature.QuickReports.List;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.QuickReports.ListV2;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<QuickReportOverviewModel>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/quick-reports:listV2");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s =>
        {
            s.Summary = "Gets all quick-reports submitted by observers for a monitoring ngo (v2)";
        });
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<PagedResponse<QuickReportOverviewModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
        SELECT
            COUNT(QR."Id") as "TotalNumberOfRows"
        FROM
            "QuickReports" QR
            INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) AMO on AMO."MonitoringObserverId" = qr."MonitoringObserverId"
        WHERE
            QR."ElectionRoundId" = @electionRoundId;

        SELECT
            QR."Id",
            QR."QuickReportLocationType",
            QR."LastUpdatedAt" AS  "Timestamp",
            QR."Title",
            QR."Description",
            QR."IncidentCategory",
            QR."FollowUpStatus",
            (SELECT COUNT(*)
                FROM "QuickReportAttachments" QRA
                WHERE QRA."QuickReportId" = QR."Id"
                  AND Qr."MonitoringObserverId" = QRA."MonitoringObserverId"
                  AND qra."IsDeleted" = FALSE
                  AND qra."IsCompleted" = TRUE) AS "NumberOfAttachments",
            AMO."MonitoringObserverId",
            AMO."DisplayName" "ObserverName",
            AMO."PhoneNumber",
            AMO."Email",
            AMO."Tags",
            AMO."NgoName",
            QR."PollingStationDetails",
            PS."Id" AS "PollingStationId",
            PS."Level1",
            PS."Level2",
            PS."Level3",
            PS."Level4",
            PS."Level5",
            PS."Number",
            PS."Address"
        FROM
            "QuickReports" QR
            INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) AMO on AMO."MonitoringObserverId" = qr."MonitoringObserverId"
            LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
        WHERE
            QR."ElectionRoundId" = @electionRoundId
        ORDER BY
            CASE WHEN @sortExpression = 'Timestamp ASC' THEN QR."LastUpdatedAt" END ASC,
            CASE WHEN @sortExpression = 'Timestamp DESC' THEN QR."LastUpdatedAt" END DESC,
            CASE WHEN @sortExpression = 'NumberOfAttachments ASC' THEN (
                SELECT COUNT(*)
                FROM "QuickReportAttachments" QRA
                WHERE QRA."QuickReportId" = QR."Id"
                  AND QR."MonitoringObserverId" = QRA."MonitoringObserverId"
                  AND QRA."IsDeleted" = FALSE
                  AND QRA."IsCompleted" = TRUE
            ) END ASC,
            CASE WHEN @sortExpression = 'NumberOfAttachments DESC' THEN (
                SELECT COUNT(*)
                FROM "QuickReportAttachments" QRA
                WHERE QRA."QuickReportId" = QR."Id"
                  AND QR."MonitoringObserverId" = QRA."MonitoringObserverId"
                  AND QRA."IsDeleted" = FALSE
                  AND QRA."IsCompleted" = TRUE
            ) END DESC
        OFFSET
            @offset ROWS
        FETCH NEXT
            @pageSize ROWS ONLY;
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString(),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<QuickReportOverviewModel> entries;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<QuickReportOverviewModel>().ToList();
        }

        return new PagedResponse<QuickReportOverviewModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(QuickReportOverviewModel.Timestamp)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(QuickReportOverviewModel.Timestamp),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(QuickReportOverviewModel.Timestamp)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(QuickReportOverviewModel.NumberOfAttachments),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(QuickReportOverviewModel.NumberOfAttachments)} {sortOrder}";
        }

        return $"{nameof(QuickReportOverviewModel.Timestamp)} ASC";
    }
}
