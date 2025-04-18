using Authorization.Policies;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Statistics.GetObserverStatistics;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IMemoryCache cache) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics:my");
        DontAutoTag();
        Options(x => x.WithTags("mobile","statistics"));
        Summary(s => { s.Summary = "Observer statistics on election round"; });
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cacheKey = $"statistics-{req.ElectionRoundId}-{req.ObserverId}";

        return await cache.GetOrCreateAsync(cacheKey, async (e) =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(120);
            return await GetObserverStatisticsAsync(req, ct);
        }) ?? new Response();
    }

    private async Task<Response> GetObserverStatisticsAsync(Request req, CancellationToken ct)
    {
        string sql = 
            """
            WITH
                "SubmissionsStats" AS (
                    SELECT
                        SUM("NumberOfFormsSubmitted") AS "NumberOfFormsSubmitted",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                    FROM
                        (
                            SELECT
                                COUNT(*) AS "NumberOfFormsSubmitted",
                                SUM(PSI."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM
                                "PollingStationInformation" PSI
                                    INNER JOIN "MonitoringObservers" MO ON PSI."MonitoringObserverId" = MO."Id"
                            WHERE
                                PSI."ElectionRoundId" = @electionRoundId
                              AND MO."ObserverId" = @observerId
                            UNION ALL
                            SELECT
                                COUNT(*) AS "NumberOfFormsSubmitted",
                                SUM(FS."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM
                                "FormSubmissions" FS
                                    INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                            WHERE
                                FS."ElectionRoundId" = @electionRoundId
                              AND MO."ObserverId" = @observerId
                        ) AS "FormsSubmittedData"
                ),
                "QuickReportStats" AS (
                    SELECT
                        COUNT(*) AS "NumberOfQuickReports"
                    FROM
                        "QuickReports" QR
                            INNER JOIN "MonitoringObservers" MO ON QR."MonitoringObserverId" = MO."Id"
                    WHERE
                        QR."ElectionRoundId" = @electionRoundId
                      AND MO."ObserverId" = @observerId
                ),
                "NoteStats" AS (
                    SELECT
                        COUNT(*) AS "NumberOfNotes"
                    FROM
                        "Notes" N
                            INNER JOIN "MonitoringObservers" MO ON N."MonitoringObserverId" = MO."Id"
                    WHERE
                        N."ElectionRoundId" = @electionRoundId
                      AND MO."ObserverId" = @observerId
                ),
                "AttachmentsStats" AS (
                    SELECT
                        SUM("NumberOfAttachments") AS "NumberOfAttachments"
                    FROM
                        (
                            SELECT
                                COUNT(*) AS "NumberOfAttachments"
                            FROM
                                "Attachments" A
                                    INNER JOIN "MonitoringObservers" MO ON A."MonitoringObserverId" = MO."Id"
                            WHERE
                                A."ElectionRoundId" = @electionRoundId
                              AND MO."ObserverId" = @observerId
                              AND A."IsDeleted" = FALSE
                            UNION ALL
                            SELECT
                                COUNT(*) AS "NumberOfAttachments"
                            FROM
                                "QuickReportAttachments" QRA
                                    INNER JOIN "MonitoringObservers" MO ON QRA."MonitoringObserverId" = MO."Id"
                            WHERE
                                QRA."ElectionRoundId" = @electionRoundId
                              AND MO."ObserverId" = @observerId
                              AND QRA."IsDeleted" = FALSE
                        ) AS "AttachmentsData"
                ),
                "PollingStationsStats" AS (
                    SELECT
                        COUNT(DISTINCT "PollingStationId") AS "NumberOfPollingStationsVisited"
                    FROM
                        (
                            SELECT
                                PSI."PollingStationId"
                            FROM
                                "PollingStationInformation" PSI
                                    INNER JOIN "MonitoringObservers" MO ON PSI."MonitoringObserverId" = MO."Id"
                            WHERE
                                PSI."ElectionRoundId" = @electionRoundId
                              AND MO."ObserverId" = @observerId
                            UNION ALL
                            SELECT
                                FS."PollingStationId"
                            FROM
                                "FormSubmissions" FS
                                    INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                            WHERE
                                FS."ElectionRoundId" = @electionRoundId
                              AND MO."ObserverId" = @observerId
                            UNION ALL
                            SELECT
                                QR."PollingStationId"
                            FROM
                                "QuickReports" QR
                                    INNER JOIN "MonitoringObservers" MO ON QR."MonitoringObserverId" = MO."Id"
                            WHERE
                                QR."ElectionRoundId" = @electionRoundId
                              AND MO."ObserverId" = @observerId
                              AND QR."PollingStationId" IS NOT NULL
                        ) AS "PollingStationsVisitData"
                )
            SELECT
                FS."NumberOfFormsSubmitted",
                FS."NumberOfQuestionsAnswered",
                QR."NumberOfQuickReports",
                N."NumberOfNotes",
                A."NumberOfAttachments",
                PS."NumberOfPollingStationsVisited"
            FROM
                "SubmissionsStats" FS,
                "QuickReportStats" QR,
                "NoteStats" N,
                "AttachmentsStats" A,
                "PollingStationsStats" PS;
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            observerId = req.ObserverId,
        };
        
        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        return await dbConnection.QueryFirstAsync<Response>(sql, queryArgs);
    }
}
