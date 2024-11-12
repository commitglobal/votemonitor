using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.MonitoringObservers.List;
public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, PagedResponse<MonitoringObserverModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Lists monitoring observers";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<PagedResponse<MonitoringObserverModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
        SELECT COUNT(*) count
        FROM
            "MonitoringObservers" MO
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
            INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
            INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
        WHERE
            MN."ElectionRoundId" = @electionRoundId
            AND MN."NgoId" = @ngoId
            AND (@searchText IS NULL OR @searchText = '' OR (U."DisplayName") ILIKE @searchText OR U."Email" ILIKE @searchText OR U."PhoneNumber" ILIKE @searchText)
            AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
            AND (@status IS NULL OR  mo."Status" = @status);

        SELECT
            "Id",
            "DisplayName",
            "PhoneNumber",
            "Email",
            "Tags",
            "Status",
            "LatestActivityAt"
        FROM (
            SELECT
                MO."Id",
                U."DisplayName",
                U."PhoneNumber",
                U."Email",
                MO."Tags",
                MO."Status",
                MAX(LATESTACTIVITY."LatestActivityAt") AS "LatestActivityAt"
            FROM
                "MonitoringObservers" MO
                INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                LEFT JOIN (
                    SELECT
                        "MonitoringObserverId",
                        MAX("LatestActivityAt") AS "LatestActivityAt"
                    FROM
                        (
                            SELECT
                                PSI."MonitoringObserverId",
                                MAX(COALESCE(PSI."LastModifiedOn", PSI."CreatedOn")) AS "LatestActivityAt"
                            FROM
                                "PollingStationInformation" PSI
                            WHERE
                                PSI."ElectionRoundId" = @electionRoundId
                            GROUP BY
                                PSI."MonitoringObserverId"
                            UNION ALL
                            SELECT
                                N."MonitoringObserverId",
                                MAX(COALESCE(N."LastModifiedOn", N."CreatedOn")) AS "LatestActivityAt"
                            FROM
                                "Notes" N
                            WHERE
                                N."ElectionRoundId" = @electionRoundId
                            GROUP BY
                                N."MonitoringObserverId"
                            UNION ALL
                            SELECT
                                A."MonitoringObserverId",
                                MAX(COALESCE(A."LastModifiedOn", A."CreatedOn")) AS "LatestActivityAt"
                            FROM
                                "Attachments" A
                            WHERE
                                A."ElectionRoundId" = @electionRoundId
                            GROUP BY
                                A."MonitoringObserverId"
                            UNION ALL
                            SELECT
                                QR."MonitoringObserverId",
                                MAX(COALESCE(QR."LastModifiedOn", QR."CreatedOn")) AS "LatestActivityAt"
                            FROM
                                "QuickReports" QR
                            WHERE
                                QR."ElectionRoundId" = @electionRoundId
                            GROUP BY
                                QR."MonitoringObserverId"
                        ) AS LATESTACTIVITYSUBQUERY
                    GROUP BY
                        "MonitoringObserverId"
                ) AS LATESTACTIVITY ON LATESTACTIVITY."MonitoringObserverId" = MO."Id"
            WHERE
                MN."ElectionRoundId" = @electionRoundId
                AND MN."NgoId" = @ngoId
                AND (@searchText IS NULL OR @searchText = '' OR (U."DisplayName") ILIKE @searchText OR U."Email" ILIKE @searchText OR u."PhoneNumber" ILIKE @searchText)
                AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
                AND (@status IS NULL OR  mo."Status" = @status)
            GROUP BY
                MO."Id",
                U."DisplayName",
                U."PhoneNumber",
                U."Email",
                MO."Tags",
                MO."Status"
            ) T

        ORDER BY
            CASE WHEN @sortExpression = 'DisplayName ASC' THEN "DisplayName" END ASC,
            CASE WHEN @sortExpression = 'DisplayName DESC' THEN "DisplayName" END DESC,

            CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN "PhoneNumber" END ASC,
            CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN "PhoneNumber" END DESC,

            CASE WHEN @sortExpression = 'Email ASC' THEN "Email" END ASC,
            CASE WHEN @sortExpression = 'Email DESC' THEN "Email" END DESC,

            CASE WHEN @sortExpression = 'Tags ASC' THEN "Tags" END ASC,
            CASE WHEN @sortExpression = 'Tags DESC' THEN "Tags" END DESC,

            CASE WHEN @sortExpression = 'LatestActivityAt ASC' THEN "LatestActivityAt" END ASC,
            CASE WHEN @sortExpression = 'LatestActivityAt DESC' THEN "LatestActivityAt" END DESC
         
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.Tags ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.Status?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<MonitoringObserverModel> entries = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<MonitoringObserverModel>().ToList();
        }
        return new PagedResponse<MonitoringObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return "DisplayName ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.DisplayName), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"DisplayName {sortOrder}";
        }
        
        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.Email), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.PhoneNumber), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.Tags), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.LatestActivityAt), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.LatestActivityAt)} {sortOrder}";
        }

        return "ObserverName ASC";
    }
}
