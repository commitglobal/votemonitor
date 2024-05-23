﻿using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.QuickReports.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<QuickReportOverviewModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s =>
        {
            s.Summary = "Gets all quick-reports submitted by observers for a monitoring ngo";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<PagedResponse<QuickReportOverviewModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT
            COUNT(QR.""Id"") as ""TotalNumberOfRows""
        FROM
            ""QuickReports"" QR
            INNER JOIN ""MonitoringObservers"" MO ON MO.""Id"" = QR.""MonitoringObserverId""
            INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
        WHERE
            QR.""ElectionRoundId"" = @electionRoundId
            AND MN.""NgoId"" = @ngoId;

        SELECT
            QR.""Id"",
            QR.""QuickReportLocationType"",
            COALESCE(QR.""LastModifiedOn"", QR.""CreatedOn"") AS  ""Timestamp"",
            QR.""Title"",
            QR.""Description"",
            QR.""FollowUpStatus"",
            COUNT(QRA.""Id"") AS ""NumberOfAttachments"",
            O.""FirstName"",
            O.""LastName"",
            O.""Email"",
            O.""PhoneNumber"",
            QR.""PollingStationDetails"",
            PS.""Id"" AS ""PollingStationId"",
            PS.""Level1"",
            PS.""Level2"",
            PS.""Level3"",
            PS.""Level4"",
            PS.""Level5"",
            PS.""Number"",
            PS.""Address""
        FROM
            ""QuickReports"" QR
            INNER JOIN ""MonitoringObservers"" MO ON MO.""Id"" = QR.""MonitoringObserverId""
            INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
            INNER JOIN ""AspNetUsers"" O ON MO.""ObserverId"" = O.""Id""
            LEFT JOIN ""QuickReportAttachments"" QRA ON QR.""Id"" = QRA.""QuickReportId""
            LEFT JOIN ""PollingStations"" PS ON PS.""Id"" = QR.""PollingStationId""
        WHERE
            QR.""ElectionRoundId"" = @electionRoundId
            AND MN.""NgoId"" = @ngoId
        GROUP BY
            QR.""Id"",
            O.""Id"",
            PS.""Id"",
            MN.""Id""
        ORDER BY
            COALESCE(QR.""LastModifiedOn"", QR.""CreatedOn"") DESC
        OFFSET
            @offset ROWS
        FETCH NEXT
            @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
        };

        int totalRowCount;
        List<QuickReportOverviewModel> entries = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<QuickReportOverviewModel>().ToList();
        }

        return new PagedResponse<QuickReportOverviewModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }
}
