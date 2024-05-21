using System.Data;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListRecipients;

public class Endpoint(IDbConnection dbConnection) :
        Endpoint<Request, PagedResponse<TargetedMonitoringObserverModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listRecipients");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<PagedResponse<TargetedMonitoringObserverModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT COUNT(*) count
        FROM
            ""MonitoringObservers"" MO
            INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
            INNER JOIN ""Observers"" O ON O.""Id"" = MO.""ObserverId""
            INNER JOIN ""AspNetUsers"" U ON U.""Id"" = O.""ApplicationUserId""
        WHERE
            MN.""ElectionRoundId"" = @electionRoundId
            AND MN.""NgoId"" = @ngoId
            AND (@searchText IS NULL OR @searchText = '' OR (U.""FirstName"" || ' ' || U.""LastName"") ILIKE @searchText OR U.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE @searchText)
            AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" @> @tagsFilter)
            AND (@status IS NULL OR  mo.""Status"" = @status)
            AND (@level1 IS NULL OR EXISTS (
                SELECT
                    1
                FROM
                    (
                        SELECT
                            PSI.""PollingStationId"" ""PollingStationId""
                        FROM
                            ""PollingStationInformation"" PSI
                            INNER JOIN ""PollingStations"" PS ON PS.""Id"" = PSI.""PollingStationId""
                        WHERE
                            PSI.""MonitoringObserverId"" = MO.""Id""
                            AND PS.""ElectionRoundId"" = @electionRoundId
                            AND PSI.""ElectionRoundId"" = @electionRoundId
                        UNION
                        SELECT
                            N.""PollingStationId"" ""PollingStationId""
                        FROM
                            ""Notes"" N
                            INNER JOIN ""PollingStations"" PS ON PS.""Id"" = N.""PollingStationId""
                        WHERE
                            N.""MonitoringObserverId"" = MO.""Id""
                            AND PS.""ElectionRoundId"" = @electionRoundId
                            AND N.""ElectionRoundId"" = @electionRoundId
                        UNION
                        SELECT
                            A.""PollingStationId"" ""PollingStationId""
                        FROM
                            ""Attachments"" A
                            INNER JOIN ""PollingStations"" PS ON PS.""Id"" = A.""PollingStationId""
                        WHERE
                            A.""MonitoringObserverId"" = MO.""Id""
                            AND PS.""ElectionRoundId"" = @electionRoundId
                            AND A.""ElectionRoundId"" = @electionRoundId
                        UNION
                        SELECT
                            QR.""PollingStationId"" ""PollingStationId""
                        FROM
                            ""QuickReports"" QR
                            INNER JOIN ""PollingStations"" PS ON PS.""Id"" = QR.""PollingStationId""
                        WHERE
                            QR.""PollingStationId"" IS NOT NULL
                            AND QR.""MonitoringObserverId"" = MO.""Id""
                            AND PS.""ElectionRoundId"" = @electionRoundId
                            AND QR.""ElectionRoundId"" = @electionRoundId
                        UNION
                        SELECT
                            FS.""PollingStationId"" ""PollingStationId""
                        FROM
                            ""FormSubmissions"" FS
                            INNER JOIN ""PollingStations"" PS ON PS.""Id"" = FS.""PollingStationId""
                        WHERE
                            FS.""MonitoringObserverId"" = MO.""Id""
                            AND PS.""ElectionRoundId"" = @electionRoundId
                            AND FS.""ElectionRoundId"" = @electionRoundId
                    ) psVisits
                    INNER JOIN ""PollingStations"" PS ON psVisits.""PollingStationId"" = PS.""Id""
                WHERE
                    ""ElectionRoundId"" = @electionRoundId
                    AND (
                        @level1 IS NULL
                        OR PS.""Level1"" = @level1
                    )
                    AND (
                        @level2 IS NULL
                        OR PS.""Level2"" = @level2
                    )
                    AND (
                        @level3 IS NULL
                        OR PS.""Level3"" = @level3
                    )
                    AND (
                        @level4 IS NULL
                        OR PS.""Level3"" = @level4
                    )
                    AND (
                        @level5 IS NULL
                        OR PS.""Level3"" = @level5
                    )));

        SELECT 
            ""MonitoringObserverId"",
            ""ObserverName"",
            ""PhoneNumber"",
            ""Email"",
            ""Tags"",
            ""Status""
        FROM (
            SELECT
                MO.""Id"" ""MonitoringObserverId"",
                U.""FirstName"" || ' ' || U.""LastName"" ""ObserverName"",
                U.""PhoneNumber"",
                U.""Email"",
                MO.""Tags"",
                MO.""Status""
            FROM
                ""MonitoringObservers"" MO
                INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
                INNER JOIN ""Observers"" O ON O.""Id"" = MO.""ObserverId""
                INNER JOIN ""AspNetUsers"" U ON U.""Id"" = O.""ApplicationUserId""
            WHERE
                MN.""ElectionRoundId"" = @electionRoundId
                AND MN.""NgoId"" = @ngoId
                AND (@searchText IS NULL OR @searchText = '' OR (U.""FirstName"" || ' ' || U.""LastName"") ILIKE @searchText OR u.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE   @searchText)
                AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" @> @tagsFilter)
                AND (@status IS NULL OR  mo.""Status"" = @status)
                AND (@level1 IS NULL OR EXISTS (
                    SELECT
                        1
                    FROM
                        (
                            SELECT
                                PSI.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""PollingStationInformation"" PSI
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = PSI.""PollingStationId""
                            WHERE
                                PSI.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND PSI.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                N.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""Notes"" N
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = N.""PollingStationId""
                            WHERE
                                N.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND N.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                A.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""Attachments"" A
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = A.""PollingStationId""
                            WHERE
                                A.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND A.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                QR.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""QuickReports"" QR
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = QR.""PollingStationId""
                            WHERE
                                QR.""PollingStationId"" IS NOT NULL
                                AND QR.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND QR.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                FS.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""FormSubmissions"" FS
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = FS.""PollingStationId""
                            WHERE
                                FS.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND FS.""ElectionRoundId"" = @electionRoundId
                        ) psVisits
                        INNER JOIN ""PollingStations"" PS ON psVisits.""PollingStationId"" = PS.""Id""
                    WHERE
                        ""ElectionRoundId"" = @electionRoundId
                        AND (
                            @level1 IS NULL
                            OR PS.""Level1"" = @level1
                        )
                        AND (
                            @level2 IS NULL
                            OR PS.""Level2"" = @level2
                        )
                        AND (
                            @level3 IS NULL
                            OR PS.""Level3"" = @level3
                        )
                        AND (
                            @level4 IS NULL
                            OR PS.""Level3"" = @level4
                        )
                        AND (
                            @level5 IS NULL
                            OR PS.""Level3"" = @level5
                        )))
            ) T

        ORDER BY
            CASE WHEN @sortExpression = 'ObserverName ASC' THEN ""ObserverName"" END ASC,
            CASE WHEN @sortExpression = 'ObserverName DESC' THEN ""ObserverName"" END DESC,

            CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN ""PhoneNumber"" END ASC,
            CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN ""PhoneNumber"" END DESC,

            CASE WHEN @sortExpression = 'Email ASC' THEN ""Email"" END ASC,
            CASE WHEN @sortExpression = 'Email DESC' THEN ""Email"" END DESC,

            CASE WHEN @sortExpression = 'Tags ASC' THEN ""Tags"" END ASC,
            CASE WHEN @sortExpression = 'Tags DESC' THEN ""Tags"" END DESC
         
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.TagsFilter ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status= req.StatusFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        };

        var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<TargetedMonitoringObserverModel>().ToList();

        return new PagedResponse<TargetedMonitoringObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.ObserverName), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} {sortOrder}";
        }
        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Email), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.PhoneNumber), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Tags), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Tags)} {sortOrder}";
        }

        return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} ASC";
    }
}
