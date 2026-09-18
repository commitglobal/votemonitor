using Authorization.Policies;
using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetNgoAdminStatistics;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IFusionCache cache) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for an election round"; });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cacheKey = $"statistics-{req.ElectionRoundId}-{req.NgoId}-{req.DataSource}";

        return await cache.GetOrSetAsync(
            cacheKey,
            async _ => await GetNgoStatistics(req, ct),
            options => options.SetDuration(StatisticsInstaller.DefaultCacheDuration),
            token: ct);
    }

    private async Task<Response> GetNgoStatistics(Request req, CancellationToken ct)
    {
        string sql = 
            """
            -- get observer stats
            SELECT
                        COUNT("MonitoringObserverId") FILTER (
                    WHERE
                    "Status" = 'Active'
                        AND "AccountStatus" = 'Active'
                    ) "ActiveObservers",
                        COUNT("MonitoringObserverId") FILTER (
                            WHERE
                            "Status" = 'Pending'
                                OR "AccountStatus" = 'Pending'
                            ) "PendingObservers",
                        COUNT("MonitoringObserverId") FILTER (
                            WHERE
                            "Status" = 'Suspended'
                                OR "AccountStatus" = 'Suspended'
                            ) "SuspendedObservers"
            FROM
                "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE);
            
            ----------------------------Levels stats--------------------------------------
            WITH
                "AvailableObservers" AS (
                    SELECT "MonitoringObserverId"
                    FROM "GetAvailableMonitoringObservers"(@ELECTIONROUNDID, @NGOID, @DATASOURCE)
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
                    WHERE PS."ElectionRoundId" = @ELECTIONROUNDID
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

            ------------------------------------------------------------------------------
            -------------------------- read hourly histogram------------------------------

            WITH "AvailableObservers" AS (
                SELECT "MonitoringObserverId"
                FROM "GetAvailableMonitoringObservers"(@ELECTIONROUNDID, @NGOID, @DATASOURCE)
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
                SELECT "MonitoringObserverId"
                FROM "GetAvailableMonitoringObservers"(@ELECTIONROUNDID, @NGOID, @DATASOURCE)
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
                INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
            WHERE CR."ElectionRoundId" = @ELECTIONROUNDID
              AND MN."NgoId" = @NGOID
            GROUP BY 1;

            WITH "AvailableObservers" AS (
                SELECT "MonitoringObserverId"
                FROM "GetAvailableMonitoringObservers"(@ELECTIONROUNDID, @NGOID, @DATASOURCE)
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
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString()
        };

        ObserversStats observersStats;
        List<VisitedPollingStationLevelStats> visitedPollingStationsStats;
        List<FormSubmissionsHistogramPoint> formSubmissionsHistogram;
        List<HistogramPoint> quickReportsHistogram;
        List<HistogramPoint> incidentReportsHistogram;
        List<HistogramPoint> citizenReportsHistogram;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using (var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs))
            {
                observersStats = multi.ReadSingle<ObserversStats>();
                visitedPollingStationsStats = multi.Read<VisitedPollingStationLevelStats>().ToList();
                formSubmissionsHistogram = multi.Read<FormSubmissionsHistogramPoint>().ToList();
                quickReportsHistogram = multi.Read<HistogramPoint>().ToList();
                incidentReportsHistogram = multi.Read<HistogramPoint>().ToList();
                citizenReportsHistogram = multi.Read<HistogramPoint>().ToList();
            }
        }

        return new Response
        {
            ObserversStats = observersStats,
            TotalStats = visitedPollingStationsStats.FirstOrDefault(x => x.Level == 0),
            Level1Stats = visitedPollingStationsStats.Where(x => x.Level == 1).ToList(),
            Level2Stats = visitedPollingStationsStats.Where(x => x.Level == 2).ToList(),
            Level3Stats = visitedPollingStationsStats.Where(x => x.Level == 3).ToList(),
            Level4Stats = visitedPollingStationsStats.Where(x => x.Level == 4).ToList(),
            Level5Stats = visitedPollingStationsStats.Where(x => x.Level == 5).ToList(),
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
