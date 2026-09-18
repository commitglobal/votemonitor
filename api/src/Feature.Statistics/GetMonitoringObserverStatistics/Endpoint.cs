using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetMonitoringObserverStatistics;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics/observers/{monitoringObserverId}");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for a specific monitoring observer"; });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var cacheKey =
            $"statistics-observer-{req.ElectionRoundId}-{req.NgoId}-{req.MonitoringObserverId}-{req.DataSource}";

        var response = await cache.GetOrSetAsync(
            cacheKey,
            async _ => await GetStatisticsAsync(req, ct),
            options => options.SetDuration(StatisticsInstaller.DefaultCacheDuration),
            token: ct);

        return TypedResults.Ok(response);
    }

    private async Task<Response> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            WITH
                "AvailableObservers" AS (
                    SELECT "MonitoringObserverId"
                    FROM "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource)
                    WHERE "MonitoringObserverId" = @monitoringObserverId
                ),
                "SubmissionsStats" AS (
                    SELECT
                        SUM("NumberOfFormsSubmitted") AS "NumberOfFormsSubmitted",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                    FROM
                        (
                            SELECT
                                COUNT(*) AS "NumberOfFormsSubmitted",
                                COALESCE(SUM(PSI."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                            FROM
                                "PollingStationInformation" PSI
                            WHERE
                                PSI."ElectionRoundId" = @electionRoundId
                              AND PSI."MonitoringObserverId" = @monitoringObserverId
                              AND EXISTS (SELECT 1 FROM "AvailableObservers")
                            UNION ALL
                            SELECT
                                COUNT(*) AS "NumberOfFormsSubmitted",
                                COALESCE(SUM(FS."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                            FROM
                                "FormSubmissions" FS
                            WHERE
                                FS."ElectionRoundId" = @electionRoundId
                              AND FS."MonitoringObserverId" = @monitoringObserverId
                              AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        ) AS "FormsSubmittedData"
                ),
                "QuickReportStats" AS (
                    SELECT COUNT(*) AS "NumberOfQuickReports"
                    FROM "QuickReports" QR
                    WHERE QR."ElectionRoundId" = @electionRoundId
                      AND QR."MonitoringObserverId" = @monitoringObserverId
                      AND EXISTS (SELECT 1 FROM "AvailableObservers")
                ),
                "NoteStats" AS (
                    SELECT COUNT(*) AS "NumberOfNotes"
                    FROM "Notes" N
                    INNER JOIN "MonitoringObservers" MO ON N."MonitoringObserverId" = MO."Id"
                    WHERE MO."ElectionRoundId" = @electionRoundId
                      AND N."MonitoringObserverId" = @monitoringObserverId
                      AND EXISTS (SELECT 1 FROM "AvailableObservers")
                ),
                "AttachmentsStats" AS (
                    SELECT COALESCE(SUM("NumberOfAttachments"), 0) AS "NumberOfAttachments"
                    FROM (
                        SELECT COUNT(*) AS "NumberOfAttachments"
                        FROM "Attachments" A
                        INNER JOIN "MonitoringObservers" MO ON A."MonitoringObserverId" = MO."Id"
                        WHERE MO."ElectionRoundId" = @electionRoundId
                          AND A."MonitoringObserverId" = @monitoringObserverId
                          AND A."IsDeleted" = FALSE
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        UNION ALL
                        SELECT COUNT(*) AS "NumberOfAttachments"
                        FROM "QuickReportAttachments" QRA
                        WHERE QRA."ElectionRoundId" = @electionRoundId
                          AND QRA."MonitoringObserverId" = @monitoringObserverId
                          AND QRA."IsDeleted" = FALSE
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                    ) AS "AttachmentsData"
                ),
                "PollingStationsStats" AS (
                    SELECT COUNT(DISTINCT "PollingStationId") AS "NumberOfPollingStationsVisited"
                    FROM (
                        SELECT PSI."PollingStationId"
                        FROM "PollingStationInformation" PSI
                        WHERE PSI."ElectionRoundId" = @electionRoundId
                          AND PSI."MonitoringObserverId" = @monitoringObserverId
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        UNION ALL
                        SELECT FS."PollingStationId"
                        FROM "FormSubmissions" FS
                        WHERE FS."ElectionRoundId" = @electionRoundId
                          AND FS."MonitoringObserverId" = @monitoringObserverId
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        UNION ALL
                        SELECT QR."PollingStationId"
                        FROM "QuickReports" QR
                        WHERE QR."ElectionRoundId" = @electionRoundId
                          AND QR."MonitoringObserverId" = @monitoringObserverId
                          AND QR."PollingStationId" IS NOT NULL
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                    ) AS "PollingStationsVisitData"
                )
            SELECT
                COALESCE(FS."NumberOfFormsSubmitted", 0) AS "NumberOfFormsSubmitted",
                COALESCE(FS."NumberOfQuestionsAnswered", 0) AS "NumberOfQuestionsAnswered",
                COALESCE(QR."NumberOfQuickReports", 0) AS "NumberOfQuickReports",
                COALESCE(N."NumberOfNotes", 0) AS "NumberOfNotes",
                COALESCE(A."NumberOfAttachments", 0) AS "NumberOfAttachments",
                COALESCE(PS."NumberOfPollingStationsVisited", 0) AS "NumberOfPollingStationsVisited"
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
            ngoId = req.NgoId,
            monitoringObserverId = req.MonitoringObserverId,
            dataSource = req.DataSource.ToString()
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        return await dbConnection.QueryFirstAsync<Response>(sql, queryArgs);
    }
}
