using Authorization.Policies;
using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetCoalitionStatistics;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}:stats");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s => { s.Summary = "Statistics for a coalition in an election round"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cacheKey = $"statistics-coalition-{req.ElectionRoundId}-{req.CoalitionId}";

        var cached = await cache.TryGetAsync<Response>(cacheKey, token: ct);
        if (cached.HasValue)
        {
            return TypedResults.Ok(cached.Value);
        }

        var result = await GetStatisticsAsync(req, ct);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        await cache.SetAsync(
            cacheKey,
            result,
            options => options.SetDuration(TimeSpan.FromMinutes(_options.CacheDurationInMinutes)),
            token: ct);

        return TypedResults.Ok(result);
    }

    private async Task<Response?> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                C."Name",
                (SELECT COUNT(*) FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = C."Id")::INT AS "NumberOfMembers"
            FROM "Coalitions" C
            WHERE C."Id" = @coalitionId
              AND C."ElectionRoundId" = @electionRoundId;
            ------------------------------

            SELECT
                COUNT(MO."Id") FILTER (
                    WHERE MO."Status" = 'Active'
                      AND U."Status" = 'Active'
                ) AS "ActiveObservers",
                COUNT(MO."Id") FILTER (
                    WHERE MO."Status" = 'Pending'
                      OR U."Status" = 'Pending'
                ) AS "PendingObservers",
                COUNT(MO."Id") FILTER (
                    WHERE MO."Status" = 'Suspended'
                      OR U."Status" = 'Deactivated'
                ) AS "SuspendedObservers"
            FROM "MonitoringObservers" MO
                INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
            WHERE MO."ElectionRoundId" = @electionRoundId
              AND MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = @coalitionId AND CM."ElectionRoundId" = @electionRoundId);
            ------------------------------

            WITH
                "AvailableObservers" AS (
                    SELECT MO."Id" AS "MonitoringObserverId"
                    FROM "MonitoringObservers" MO
                    WHERE MO."ElectionRoundId" = @electionRoundId
                      AND MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = @coalitionId AND CM."ElectionRoundId" = @electionRoundId)
                ),
                "ActiveObservers" AS (
                    SELECT
                        FS."PollingStationId",
                        FS."MonitoringObserverId"
                    FROM "FormSubmissions" FS
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                    UNION
                    SELECT
                        PSI."PollingStationId",
                        PSI."MonitoringObserverId"
                    FROM "PollingStationInformation" PSI
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                    UNION
                    SELECT
                        QR."PollingStationId",
                        QR."MonitoringObserverId"
                    FROM "QuickReports" QR
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                    WHERE QR."PollingStationId" IS NOT NULL
                    UNION
                    SELECT
                        IR."PollingStationId",
                        IR."MonitoringObserverId"
                    FROM "IncidentReports" IR
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                    WHERE IR."PollingStationId" IS NOT NULL
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
                ),
                "ActiveObserversPerLevel" AS (
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
                        COUNT(DISTINCT AO."MonitoringObserverId") AS "ActiveObservers"
                    FROM "ActiveObservers" AO
                        INNER JOIN "ElectionPollingStations" PS ON PS."Id" = AO."PollingStationId"
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
                AOL."ActiveObservers",
                (
                    S."NumberOfVisitedPollingStations" * 100.0 / NULLIF(PS."NumberOfPollingStations", 0)
                ) AS "CoveragePercentage"
            FROM "PollingStationLevelsStats" S
                INNER JOIN "PollingStationsPerLevel" PS ON S."Level" = PS."Level" AND S."Path" = PS."Path"
                INNER JOIN "ActiveObserversPerLevel" AOL ON S."Level" = AOL."Level" AND S."Path" = AOL."Path";

            WITH
                "LocationStatsRaw" AS (
                    SELECT
                        CR."LocationId",
                        COUNT(1) AS "NumberOfCitizenReports",
                        SUM(CR."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM(CR."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                    FROM "CitizenReports" CR
                        INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                    WHERE CR."ElectionRoundId" = @electionRoundId
                      AND ER."MonitoringNgoForCitizenReportingId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = @coalitionId AND CM."ElectionRoundId" = @electionRoundId)
                      AND CR."NumberOfQuestionsAnswered" > 0
                    GROUP BY CR."LocationId"
                ),
                "ElectionLocations" AS (
                    SELECT
                        L."Id",
                        NULLIF(L."Level1", '') AS "Level1",
                        NULLIF(L."Level2", '') AS "Level2",
                        NULLIF(L."Level3", '') AS "Level3",
                        NULLIF(L."Level4", '') AS "Level4",
                        NULLIF(L."Level5", '') AS "Level5"
                    FROM "Locations" L
                    WHERE L."ElectionRoundId" = @electionRoundId
                      AND L."Level1" != ''
                ),
                "LocationsPerLevel" AS (
                    SELECT
                        CASE
                            WHEN GROUPING(L."Level1") = 1 THEN 0
                            WHEN GROUPING(L."Level2") = 1 THEN 1
                            WHEN GROUPING(L."Level3") = 1 THEN 2
                            WHEN GROUPING(L."Level4") = 1 THEN 3
                            WHEN GROUPING(L."Level5") = 1 THEN 4
                            ELSE 5
                        END AS "Level",
                        CASE
                            WHEN GROUPING(L."Level1") = 1 THEN '/'
                            ELSE CONCAT_WS(' / ', L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                        END AS "Path",
                        COUNT(L."Id") AS "NumberOfLocations"
                    FROM "ElectionLocations" L
                    GROUP BY GROUPING SETS (
                        (),
                        (L."Level1"),
                        (L."Level1", L."Level2"),
                        (L."Level1", L."Level2", L."Level3"),
                        (L."Level1", L."Level2", L."Level3", L."Level4"),
                        (L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                    )
                    HAVING
                        GROUPING(L."Level1") = 1
                        OR (
                            GROUPING(L."Level2") = 1
                            AND L."Level1" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level3") = 1
                            AND GROUPING(L."Level2") = 0
                            AND L."Level2" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level4") = 1
                            AND GROUPING(L."Level3") = 0
                            AND L."Level3" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level5") = 1
                            AND GROUPING(L."Level4") = 0
                            AND L."Level4" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level5") = 0
                            AND L."Level5" IS NOT NULL
                        )
                ),
                "LocationLevelsStats" AS (
                    SELECT
                        CASE
                            WHEN GROUPING(L."Level1") = 1 THEN 0
                            WHEN GROUPING(L."Level2") = 1 THEN 1
                            WHEN GROUPING(L."Level3") = 1 THEN 2
                            WHEN GROUPING(L."Level4") = 1 THEN 3
                            WHEN GROUPING(L."Level5") = 1 THEN 4
                            ELSE 5
                        END AS "Level",
                        CASE
                            WHEN GROUPING(L."Level1") = 1 THEN '/'
                            ELSE CONCAT_WS(' / ', L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                        END AS "Path",
                        COUNT(LS."LocationId") AS "NumberOfVisitedLocations",
                        COALESCE(SUM(LS."NumberOfCitizenReports"), 0) AS "NumberOfCitizenReports",
                        COALESCE(SUM(LS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                        COALESCE(SUM(LS."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                    FROM "LocationStatsRaw" LS
                        INNER JOIN "ElectionLocations" L ON L."Id" = LS."LocationId"
                    GROUP BY GROUPING SETS (
                        (),
                        (L."Level1"),
                        (L."Level1", L."Level2"),
                        (L."Level1", L."Level2", L."Level3"),
                        (L."Level1", L."Level2", L."Level3", L."Level4"),
                        (L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                    )
                    HAVING
                        GROUPING(L."Level1") = 1
                        OR (
                            GROUPING(L."Level2") = 1
                            AND L."Level1" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level3") = 1
                            AND GROUPING(L."Level2") = 0
                            AND L."Level2" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level4") = 1
                            AND GROUPING(L."Level3") = 0
                            AND L."Level3" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level5") = 1
                            AND GROUPING(L."Level4") = 0
                            AND L."Level4" IS NOT NULL
                        )
                        OR (
                            GROUPING(L."Level5") = 0
                            AND L."Level5" IS NOT NULL
                        )
                )
            SELECT
                S."Path",
                S."Level",
                S."NumberOfVisitedLocations",
                L."NumberOfLocations",
                S."NumberOfCitizenReports",
                S."NumberOfFlaggedAnswers",
                S."NumberOfQuestionsAnswered",
                (
                    S."NumberOfVisitedLocations" * 100.0 / NULLIF(L."NumberOfLocations", 0)
                ) AS "CoveragePercentage"
            FROM "LocationLevelsStats" S
                INNER JOIN "LocationsPerLevel" L ON S."Level" = L."Level" AND S."Path" = L."Path";

            WITH "AvailableObservers" AS (
                SELECT MO."Id" AS "MonitoringObserverId"
                FROM "MonitoringObservers" MO
                WHERE MO."ElectionRoundId" = @electionRoundId
                  AND MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = @coalitionId AND CM."ElectionRoundId" = @electionRoundId)
            ),
            submission_histogram AS (
                SELECT
                    DATE_TRUNC(
                            'hour',
                            TIMEZONE('utc', FS."LastUpdatedAt")
                    )::timestamptz AS bucket,
                    1 AS forms_submitted,
                    FS."NumberOfQuestionsAnswered" AS number_of_questions_answered,
                    FS."NumberOfFlaggedAnswers" AS number_of_flagged_answers
                FROM "FormSubmissions" FS
                    INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"

                UNION ALL

                SELECT
                    DATE_TRUNC(
                            'hour',
                            TIMEZONE('utc', qr."LastUpdatedAt")
                    )::timestamptz AS bucket,
                    1 AS forms_submitted,
                    qr."NumberOfQuestionsAnswered" AS number_of_questions_answered,
                    qr."NumberOfFlaggedAnswers" AS number_of_flagged_answers
                FROM "PollingStationInformation" qr
                    INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = qr."MonitoringObserverId"
            )
            SELECT
                bucket AS "Bucket",
                COUNT(*) AS "FormsSubmitted",
                SUM(number_of_questions_answered) AS "NumberOfQuestionsAnswered",
                SUM(number_of_flagged_answers) AS "NumberOfFlaggedAnswers"
            FROM submission_histogram
            GROUP BY bucket
            ORDER BY bucket;

            WITH "AvailableObservers" AS (
                SELECT MO."Id" AS "MonitoringObserverId"
                FROM "MonitoringObservers" MO
                WHERE MO."ElectionRoundId" = @electionRoundId
                  AND MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = @coalitionId AND CM."ElectionRoundId" = @electionRoundId)
            )
            SELECT
                DATE_TRUNC(
                        'hour',
                        TIMEZONE('utc', QR."LastUpdatedAt")
                )::TIMESTAMPTZ "Bucket",
                COUNT(1) "Value"
            FROM "QuickReports" QR
                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
            GROUP BY 1;

            SELECT
                DATE_TRUNC(
                        'hour',
                        TIMEZONE(
                                'utc',
                                COALESCE(CR."LastModifiedOn", CR."CreatedOn")
                        )
                )::TIMESTAMPTZ "Bucket",
                COUNT(1) "Value"
            FROM "CitizenReports" CR
                INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
            WHERE CR."ElectionRoundId" = @electionRoundId
              AND ER."MonitoringNgoForCitizenReportingId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = @coalitionId AND CM."ElectionRoundId" = @electionRoundId)
            GROUP BY 1;

            WITH "AvailableObservers" AS (
                SELECT MO."Id" AS "MonitoringObserverId"
                FROM "MonitoringObservers" MO
                WHERE MO."ElectionRoundId" = @electionRoundId
                  AND MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = @coalitionId AND CM."ElectionRoundId" = @electionRoundId)
            )
            SELECT
                DATE_TRUNC(
                        'hour',
                        TIMEZONE('utc', IR."LastUpdatedAt")
                )::TIMESTAMPTZ "Bucket",
                COUNT(1) "Value"
            FROM "IncidentReports" IR
                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
            GROUP BY 1;
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            coalitionId = req.CoalitionId
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

        var coalition = multi.Read<CoalitionHeader>().SingleOrDefault();
        if (coalition is null)
        {
            return null;
        }

        var observersStats = multi.ReadSingle<ObserversStats>();
        var visitedPollingStationsStats = multi.Read<VisitedPollingStationLevelStats>().ToList();
        var visitedLocationStats = multi.Read<VisitedLocationLevelStats>().ToList();
        var formSubmissionsHistogram = multi.Read<FormSubmissionsHistogramPoint>().ToList();
        var quickReportsHistogram = multi.Read<HistogramPoint>().ToList();
        var citizenReportsHistogram = multi.Read<HistogramPoint>().ToList();
        var incidentReportsHistogram = multi.Read<HistogramPoint>().ToList();

        return new Response
        {
            Name = coalition.Name,
            NumberOfMembers = coalition.NumberOfMembers,
            ObserversStats = observersStats,
            TotalStats = visitedPollingStationsStats.FirstOrDefault(x => x.Level == 0),
            Level1Stats = visitedPollingStationsStats.Where(x => x.Level == 1).ToList(),
            Level2Stats = visitedPollingStationsStats.Where(x => x.Level == 2).ToList(),
            Level3Stats = visitedPollingStationsStats.Where(x => x.Level == 3).ToList(),
            Level4Stats = visitedPollingStationsStats.Where(x => x.Level == 4).ToList(),
            Level5Stats = visitedPollingStationsStats.Where(x => x.Level == 5).ToList(),
            TotalLocationStats = visitedLocationStats.FirstOrDefault(x => x.Level == 0),
            LocationLevel1Stats = visitedLocationStats.Where(x => x.Level == 1).ToList(),
            LocationLevel2Stats = visitedLocationStats.Where(x => x.Level == 2).ToList(),
            LocationLevel3Stats = visitedLocationStats.Where(x => x.Level == 3).ToList(),
            LocationLevel4Stats = visitedLocationStats.Where(x => x.Level == 4).ToList(),
            LocationLevel5Stats = visitedLocationStats.Where(x => x.Level == 5).ToList(),
            FormsHistogram = formSubmissionsHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.FormsSubmitted
            }).ToArray(),
            QuestionsHistogram = formSubmissionsHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfQuestionsAnswered
            }).ToArray(),
            FlaggedAnswersHistogram = formSubmissionsHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfFlaggedAnswers
            }).ToArray(),
            QuickReportsHistogram = quickReportsHistogram.ToArray(),
            IncidentReportsHistogram = incidentReportsHistogram.ToArray(),
            CitizenReportsHistogram = citizenReportsHistogram.ToArray()
        };
    }
}
