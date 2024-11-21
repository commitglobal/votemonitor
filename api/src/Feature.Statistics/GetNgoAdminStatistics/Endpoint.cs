using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Microsoft.Extensions.Caching.Memory;
using NPOI.POIFS.NIO;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Statistics.GetNgoAdminStatistics;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IMemoryCache cache) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for an election round"; });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cacheKey = $"statistics-{req.ElectionRoundId}-{req.NgoId}-{req.DataSource}";

        return await cache.GetOrCreateAsync(cacheKey, async (e) =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await GetNgoStatistics(req, ct);
        }) ?? new Response();
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
                "ActiveObservers" AS (
                    SELECT
                        "PollingStationId",
                        FS."MonitoringObserverId"
                    FROM
                        "FormSubmissions" FS
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                    UNION
                    SELECT
                        "PollingStationId",
                        PSI."MonitoringObserverId"
                    FROM
                        "PollingStationInformation" PSI
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                    UNION
                    SELECT
                        "PollingStationId",
                        QR."MonitoringObserverId"
                    FROM
                        "QuickReports" QR
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                    WHERE
                        QR."PollingStationId" IS NOT NULL
                    UNION
                    SELECT
                        "PollingStationId",
                        IR."MonitoringObserverId"
                    FROM
                        "IncidentReports" IR
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                    WHERE
                        IR."PollingStationId" IS NOT NULL
                ),
                "ActiveObserversPerLevel" AS (
                    SELECT
                        '/' AS "Path",
                        0 AS "Level",
                        COUNT(DISTINCT AO."MonitoringObserverId") "ActiveObservers"
                    FROM
                        "ActiveObservers" AO
                    UNION ALL
                    SELECT
                        PS."Level1" AS "Path",
                        1 AS "Level",
                        COUNT(DISTINCT AO."MonitoringObserverId") "ActiveObservers"
                    FROM
                        "ActiveObservers" AO
                            INNER JOIN "PollingStations" PS ON PS."Id" = AO."PollingStationId"
                    WHERE
                        PS."Level1" != ''
                    GROUP BY
                        PS."Level1"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" AS "Path",
                        2 AS "Level",
                        COUNT(DISTINCT AO."MonitoringObserverId") "ActiveObservers"
                    FROM
                        "ActiveObservers" AO
                            INNER JOIN "PollingStations" PS ON PS."Id" = AO."PollingStationId"
                    WHERE
                        PS."Level2" IS NOT NULL
                      AND PS."Level2" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" AS "Path",
                        3 AS "Level",
                        COUNT(DISTINCT AO."MonitoringObserverId") "ActiveObservers"
                    FROM
                        "ActiveObservers" AO
                            INNER JOIN "PollingStations" PS ON PS."Id" = AO."PollingStationId"
                    WHERE
                        PS."Level3" IS NOT NULL
                      AND PS."Level3" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" AS "Path",
                        4 AS "Level",
                        COUNT(DISTINCT AO."MonitoringObserverId") "ActiveObservers"
                    FROM
                        "ActiveObservers" AO
                            INNER JOIN "PollingStations" PS ON PS."Id" = AO."PollingStationId"
                    WHERE
                        PS."Level4" IS NOT NULL
                      AND PS."Level4" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" || ' / ' || PS."Level5" AS "Path",
                        5 AS "Level",
                        COUNT(DISTINCT AO."MonitoringObserverId") "ActiveObservers"
                    FROM
                        "ActiveObservers" AO
                            INNER JOIN "PollingStations" PS ON PS."Id" = AO."PollingStationId"
                    WHERE
                        PS."Level5" IS NOT NULL
                      AND PS."Level5" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" || ' / ' || PS."Level5"
                ),
                "PollingStationsStats" AS (
                    SELECT
                        "PollingStationId",
                        0 AS "NumberOfIncidentReports",
                        0 AS "NumberOfQuickReports",
                        COUNT(1) AS "NumberOfSubmissions",
                        0 AS "MinutesMonitoring",
                        SUM(FS."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM(FS."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        COUNT(FS."MonitoringObserverId") AS "ActiveObservers"
                    FROM
                        "FormSubmissions" FS
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                    GROUP BY
                        "PollingStationId"
                    UNION ALL
                    SELECT
                        "PollingStationId",
                        0 AS "NumberOfIncidentReports",
                        0 AS "NumberOfQuickReports",
                        COUNT(1) AS "NumberOfSubmissions",
                        COALESCE(SUM(COALESCE(PSI."MinutesMonitoring", 0)), 0) AS "MinutesMonitoring",
                        SUM(PSI."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM(PSI."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        COUNT(PSI."MonitoringObserverId") AS "ActiveObservers"
                    FROM
                        "PollingStationInformation" PSI
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                    GROUP BY
                        "PollingStationId"
                    UNION ALL
                    SELECT
                        "PollingStationId",
                        0 AS "NumberOfIncidentReports",
                        COUNT(1) AS "NumberOfQuickReports",
                        0 AS "NumberOfSubmissions",
                        0 AS "MinutesMonitoring",
                        0 AS "NumberOfFlaggedAnswers",
                        0 AS "NumberOfQuestionsAnswered",
                        COUNT(QR."MonitoringObserverId") AS "ActiveObservers"
                    FROM
                        "QuickReports" QR
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                    GROUP BY
                        "PollingStationId"
                    UNION ALL
                    SELECT
                        "PollingStationId",
                        COUNT(1) AS "NumberOfIncidentReports",
                        0 AS "NumberOfQuickReports",
                        0 AS "NumberOfSubmissions",
                        0 AS "MinutesMonitoring",
                        0 AS "NumberOfFlaggedAnswers",
                        0 AS "NumberOfQuestionsAnswered",
                        COUNT(IR."MonitoringObserverId") AS "ActiveObservers"
                    FROM
                        "IncidentReports" IR
                            INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                    GROUP BY
                        "PollingStationId"
                ),
                "PollingStationLevelsStats" AS (
                    SELECT
                        '/' AS "Path",
                        COUNT(DISTINCT PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                        SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                        SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                        SUM("NumberOfSubmissions") AS "NumberOfSubmissions",
                        SUM("MinutesMonitoring") AS "MinutesMonitoring",
                        SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        0 AS "Level"
                    FROM
                        "PollingStationsStats" PSV
                    UNION ALL
                    SELECT
                        PS."Level1" AS "Path",
                        COUNT(DISTINCT PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                        SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                        SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                        SUM("NumberOfSubmissions") AS "NumberOfSubmissions",
                        SUM("MinutesMonitoring") AS "MinutesMonitoring",
                        SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        1 AS "Level"
                    FROM
                        "PollingStationsStats" PSV
                            INNER JOIN "PollingStations" PS ON PSV."PollingStationId" = PS."Id"
                    WHERE
                        PS."Level1" != ''
                    GROUP BY
                        PS."Level1"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" AS "Path",
                        COUNT(DISTINCT PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                        SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                        SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                        SUM("NumberOfSubmissions") AS "NumberOfSubmissions",
                        SUM("MinutesMonitoring") AS "MinutesMonitoring",
                        SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        2 AS "Level"
                    FROM
                        "PollingStationsStats" PSV
                            INNER JOIN "PollingStations" PS ON PSV."PollingStationId" = PS."Id"
                            INNER JOIN "ActiveObservers" AO ON AO."PollingStationId" = PS."Id"
                    WHERE
                        PS."Level2" IS NOT NULL
                      AND PS."Level2" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" AS "Path",
                        COUNT(DISTINCT PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                        SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                        SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                        SUM("NumberOfSubmissions") AS "NumberOfSubmissions",
                        SUM("MinutesMonitoring") AS "MinutesMonitoring",
                        SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        3 AS "Level"
                    FROM
                        "PollingStationsStats" PSV
                            INNER JOIN "PollingStations" PS ON PSV."PollingStationId" = PS."Id"
                            INNER JOIN "ActiveObservers" AO ON AO."PollingStationId" = PS."Id"
                    WHERE
                        PS."Level3" IS NOT NULL
                      AND PS."Level3" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" AS "Path",
                        COUNT(DISTINCT PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                        SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                        SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                        SUM("NumberOfSubmissions") AS "NumberOfSubmissions",
                        SUM("MinutesMonitoring") AS "MinutesMonitoring",
                        SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        4 AS "Level"
                    FROM
                        "PollingStationsStats" PSV
                            INNER JOIN "PollingStations" PS ON PSV."PollingStationId" = PS."Id"
                            INNER JOIN "ActiveObservers" AO ON AO."PollingStationId" = PS."Id"
                    WHERE
                        PS."Level4" IS NOT NULL
                      AND PS."Level4" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" || ' / ' || PS."Level5" AS "Path",
                        COUNT(DISTINCT PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                        SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                        SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                        SUM("NumberOfSubmissions") AS "NumberOfSubmissions",
                        SUM("MinutesMonitoring") AS "MinutesMonitoring",
                        SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                        SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                        5 AS "Level"
                    FROM
                        "PollingStationsStats" PSV
                            INNER JOIN "PollingStations" PS ON PSV."PollingStationId" = PS."Id"
                            INNER JOIN "ActiveObservers" AO ON AO."PollingStationId" = PS."Id"
                    WHERE
                        PS."Level5" IS NOT NULL
                      AND PS."Level5" != ''
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" || ' / ' || PS."Level5"
                ),
                "PollingStationsPerLevel" AS (
                    SELECT
                        '/' AS "Path",
                        COUNT(PS."Id") AS "NumberOfPollingStations",
                        0 AS "Level"
                    FROM
                        "PollingStations" PS
                    WHERE
                        PS."Level1" != ''
                      AND PS."ElectionRoundId" = @ELECTIONROUNDID
                    UNION ALL
                    SELECT
                        PS."Level1" AS "Path",
                        COUNT(PS."Id") AS "NumberOfPollingStations",
                        1 AS "Level"
                    FROM
                        "PollingStations" PS
                    WHERE
                        PS."Level1" != ''
                      AND PS."ElectionRoundId" = @ELECTIONROUNDID
                    GROUP BY
                        PS."Level1"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" AS "Path",
                        COUNT(PS."Id") AS "NumberOfPollingStations",
                        2 AS "Level"
                    FROM
                        "PollingStations" PS
                    WHERE
                        "Level2" IS NOT NULL
                      AND "Level2" != ''
                      AND PS."ElectionRoundId" = @ELECTIONROUNDID
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" AS "Path",
                        COUNT(PS."Id") AS "NumberOfPollingStations",
                        3 AS "Level"
                    FROM
                        "PollingStations" PS
                    WHERE
                        "Level3" IS NOT NULL
                      AND "Level3" != ''
                      AND PS."ElectionRoundId" = @ELECTIONROUNDID
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" AS "Path",
                        COUNT(PS."Id") AS "NumberOfPollingStations",
                        4 AS "Level"
                    FROM
                        "PollingStations" PS
                    WHERE
                        "Level4" IS NOT NULL
                      AND "Level4" != ''
                      AND PS."ElectionRoundId" = @ELECTIONROUNDID
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4"
                    UNION ALL
                    SELECT
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" || ' / ' || PS."Level5" AS "Path",
                        COUNT(PS."Id") AS "NumberOfPollingStations",
                        5 AS "Level"
                    FROM
                        "PollingStations" PS
                    WHERE
                        "Level5" IS NOT NULL
                      AND "Level5" != ''
                      AND PS."ElectionRoundId" = @ELECTIONROUNDID
                    GROUP BY
                        PS."Level1" || ' / ' || PS."Level2" || ' / ' || PS."Level3" || ' / ' || PS."Level4" || ' / ' || PS."Level5"
                ),
                "AllPollingStationLevelsStats" AS (
                    SELECT
                        S.*,
                        PS."NumberOfPollingStations" AS "NumberOfPollingStations"
                    FROM
                        "PollingStationLevelsStats" S
                            INNER JOIN "PollingStationsPerLevel" PS ON S."Level" = PS."Level"
                            AND S."Path" = PS."Path"
                )
            SELECT
                A.*,
                AOL."ActiveObservers" AS "ActiveObservers",
                (
                    A."NumberOfVisitedPollingStations" * 100.0 / NULLIF(A."NumberOfPollingStations", 0)
                    ) AS "CoveragePercentage"
            FROM
                "AllPollingStationLevelsStats" A
                    INNER JOIN "ActiveObserversPerLevel" AOL ON A."Level" = AOL."Level"
                    AND A."Path" = AOL."Path";
            
            ------------------------------------------------------------------------------
            -------------------------- read hourly histogram------------------------------
            SELECT
                DATE_TRUNC(
                        'hour',
                        TIMEZONE (
                                'utc',
                                COALESCE(FS."LastModifiedOn", FS."CreatedOn")
                        )
                )::TIMESTAMPTZ AS "Bucket",
                COUNT(1) AS "FormsSubmitted",
                SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered",
                SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers"
            FROM
                "FormSubmissions" FS
                    INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
            GROUP BY
                1;
            
            SELECT
                DATE_TRUNC(
                        'hour',
                        TIMEZONE (
                                'utc',
                                COALESCE(QR."LastModifiedOn", QR."CreatedOn")
                        )
                )::TIMESTAMPTZ "Bucket",
                COUNT(1) "Value"
            FROM
                "QuickReports" QR
                    INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
            GROUP BY
                1;
            
            SELECT
                DATE_TRUNC(
                        'hour',
                        TIMEZONE (
                                'utc',
                                COALESCE(CR."LastModifiedOn", CR."CreatedOn")
                        )
                )::TIMESTAMPTZ "Bucket",
                COUNT(1) "Value"
            FROM
                "CitizenReports" CR
                    INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
            WHERE
                CR."ElectionRoundId" = @ELECTIONROUNDID
              AND MN."NgoId" = @NGOID
            GROUP BY
                1;
            
            SELECT
                DATE_TRUNC(
                        'hour',
                        TIMEZONE (
                                'utc',
                                COALESCE(IR."LastModifiedOn", IR."CreatedOn")
                        )
                )::TIMESTAMPTZ "Bucket",
                COUNT(1) "Value"
            FROM
                "IncidentReports" IR
                    INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
            GROUP BY
                1;
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
