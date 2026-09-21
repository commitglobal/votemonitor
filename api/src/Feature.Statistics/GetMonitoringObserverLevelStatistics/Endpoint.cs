using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Feature.Statistics.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetMonitoringObserverLevelStatistics;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-observers/{monitoringObserverId}:statistics");
        DontAutoTag();
        Options(x => x.WithTags("statistics", "monitoring-observers"));
        Summary(s => { s.Summary = "Location-level statistics for a specific monitoring observer"; });
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
            $"monitoring-observer-level-stats-{req.ElectionRoundId}-{req.NgoId}-{req.MonitoringObserverId}-{req.DataSource}";

        var response = await cache.GetOrSetAsync(
            cacheKey,
            async _ => await GetStatisticsAsync(req, ct),
            cacheOptions => cacheOptions.SetDuration(TimeSpan.FromMinutes(_options.CacheDurationInMinutes)),
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
                "PollingStationsStatsRaw" AS (
                    SELECT
                        FS."PollingStationId",
                        0 AS "NumberOfIncidentReports",
                        0 AS "NumberOfQuickReports",
                        COUNT(1) AS "NumberOfFormSubmissions",
                        0::float AS "MinutesMonitoring",
                        SUM(FS."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM(FS."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                    FROM "FormSubmissions" FS
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                    GROUP BY FS."PollingStationId"
                    UNION ALL
                    SELECT
                        PSI."PollingStationId",
                        0 AS "NumberOfIncidentReports",
                        0 AS "NumberOfQuickReports",
                        COUNT(1) AS "NumberOfFormSubmissions",
                        SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")) AS "MinutesMonitoring",
                        SUM(PSI."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM(PSI."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                    FROM "PollingStationInformation" PSI
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                    GROUP BY PSI."PollingStationId"
                    UNION ALL
                    SELECT
                        QR."PollingStationId",
                        0 AS "NumberOfIncidentReports",
                        COUNT(1) AS "NumberOfQuickReports",
                        0 AS "NumberOfFormSubmissions",
                        0::float AS "MinutesMonitoring",
                        0 AS "NumberOfFlaggedAnswers",
                        0 AS "NumberOfQuestionsAnswered"
                    FROM "QuickReports" QR
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                    WHERE QR."PollingStationId" IS NOT NULL
                    GROUP BY QR."PollingStationId"
                    UNION ALL
                    SELECT
                        IR."PollingStationId",
                        COUNT(1) AS "NumberOfIncidentReports",
                        0 AS "NumberOfQuickReports",
                        0 AS "NumberOfFormSubmissions",
                        0::float AS "MinutesMonitoring",
                        0 AS "NumberOfFlaggedAnswers",
                        0 AS "NumberOfQuestionsAnswered"
                    FROM "IncidentReports" IR
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                    WHERE IR."PollingStationId" IS NOT NULL
                    GROUP BY IR."PollingStationId"
                ),
                "PollingStationsStats" AS (
                    SELECT
                        "PollingStationId",
                        SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                        SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                        SUM("NumberOfFormSubmissions") AS "NumberOfFormSubmissions",
                        SUM("MinutesMonitoring") AS "MinutesMonitoring",
                        SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                    FROM "PollingStationsStatsRaw"
                    GROUP BY "PollingStationId"
                ),
                "ElectionPollingStations" AS (
                    SELECT
                        PS."Id",
                        NULLIF(PS."Level1", '') AS "Level1",
                        NULLIF(PS."Level2", '') AS "Level2",
                        NULLIF(PS."Level3", '') AS "Level3",
                        NULLIF(PS."Level4", '') AS "Level4",
                        NULLIF(PS."Level5", '') AS "Level5"
                    FROM "PollingStations" PS
                    WHERE PS."ElectionRoundId" = @electionRoundId
                      AND PS."Level1" != ''
                ),
                "PollingStationsPerLevel" AS (
                    SELECT
                        CASE
                            WHEN GROUPING(PS."Level1") = 1 THEN 0
                            WHEN GROUPING(PS."Level2") = 1 THEN 1
                            WHEN GROUPING(PS."Level3") = 1 THEN 2
                            WHEN GROUPING(PS."Level4") = 1 THEN 3
                            WHEN GROUPING(PS."Level5") = 1 THEN 4
                            ELSE 5
                        END AS "Level",
                        CASE
                            WHEN GROUPING(PS."Level1") = 1 THEN '/'
                            ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                        END AS "Path",
                        COUNT(PS."Id") AS "NumberOfPollingStations"
                    FROM "ElectionPollingStations" PS
                    GROUP BY GROUPING SETS (
                        (),
                        (PS."Level1"),
                        (PS."Level1", PS."Level2"),
                        (PS."Level1", PS."Level2", PS."Level3"),
                        (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                        (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                    )
                    HAVING
                        GROUPING(PS."Level1") = 1
                        OR (
                            GROUPING(PS."Level2") = 1
                            AND PS."Level1" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level3") = 1
                            AND GROUPING(PS."Level2") = 0
                            AND PS."Level2" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level4") = 1
                            AND GROUPING(PS."Level3") = 0
                            AND PS."Level3" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level5") = 1
                            AND GROUPING(PS."Level4") = 0
                            AND PS."Level4" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level5") = 0
                            AND PS."Level5" IS NOT NULL
                        )
                ),
                "PollingStationLevelsStats" AS (
                    SELECT
                        CASE
                            WHEN GROUPING(PS."Level1") = 1 THEN 0
                            WHEN GROUPING(PS."Level2") = 1 THEN 1
                            WHEN GROUPING(PS."Level3") = 1 THEN 2
                            WHEN GROUPING(PS."Level4") = 1 THEN 3
                            WHEN GROUPING(PS."Level5") = 1 THEN 4
                            ELSE 5
                        END AS "Level",
                        CASE
                            WHEN GROUPING(PS."Level1") = 1 THEN '/'
                            ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                        END AS "Path",
                        COUNT(PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                        COALESCE(SUM(PSV."NumberOfIncidentReports"), 0) AS "NumberOfIncidentReports",
                        COALESCE(SUM(PSV."NumberOfQuickReports"), 0) AS "NumberOfQuickReports",
                        COALESCE(SUM(PSV."NumberOfFormSubmissions"), 0) AS "NumberOfFormSubmissions",
                        COALESCE(SUM(PSV."MinutesMonitoring"), 0) AS "MinutesMonitoring",
                        COALESCE(SUM(PSV."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                        COALESCE(SUM(PSV."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                    FROM "PollingStationsStats" PSV
                        INNER JOIN "ElectionPollingStations" PS ON PS."Id" = PSV."PollingStationId"
                    GROUP BY GROUPING SETS (
                        (),
                        (PS."Level1"),
                        (PS."Level1", PS."Level2"),
                        (PS."Level1", PS."Level2", PS."Level3"),
                        (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                        (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                    )
                    HAVING
                        GROUPING(PS."Level1") = 1
                        OR (
                            GROUPING(PS."Level2") = 1
                            AND PS."Level1" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level3") = 1
                            AND GROUPING(PS."Level2") = 0
                            AND PS."Level2" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level4") = 1
                            AND GROUPING(PS."Level3") = 0
                            AND PS."Level3" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level5") = 1
                            AND GROUPING(PS."Level4") = 0
                            AND PS."Level4" IS NOT NULL
                        )
                        OR (
                            GROUPING(PS."Level5") = 0
                            AND PS."Level5" IS NOT NULL
                        )
                )
            SELECT
                S."Path",
                S."Level",
                S."NumberOfVisitedPollingStations",
                PS."NumberOfPollingStations",
                S."NumberOfIncidentReports",
                S."NumberOfQuickReports",
                S."NumberOfFormSubmissions",
                S."MinutesMonitoring",
                S."NumberOfFlaggedAnswers",
                S."NumberOfQuestionsAnswered",
                CASE WHEN S."NumberOfVisitedPollingStations" > 0 THEN 1 ELSE 0 END AS "ActiveObservers",
                (
                    S."NumberOfVisitedPollingStations" * 100.0 / NULLIF(PS."NumberOfPollingStations", 0)
                ) AS "CoveragePercentage"
            FROM "PollingStationLevelsStats" S
                INNER JOIN "PollingStationsPerLevel" PS ON S."Level" = PS."Level" AND S."Path" = PS."Path";
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            monitoringObserverId = req.MonitoringObserverId,
            dataSource = req.DataSource.ToString()
        };

        List<VisitedPollingStationLevelStats> levelStats;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            levelStats = (await dbConnection.QueryAsync<VisitedPollingStationLevelStats>(sql, queryArgs)).ToList();
        }

        return new Response
        {
            Levels = levelStats
                .Where(x => x.Level is >= 1 and <= 5)
                .GroupBy(x => x.Level)
                .OrderBy(g => g.Key)
                .Select(g => new LevelStatistics
                {
                    Level = g.Key,
                    Stats = g.OrderBy(s => s.Path).ToList()
                })
                .Where(l => l.Stats.Count > 0)
                .ToList()
        };
    }
}
