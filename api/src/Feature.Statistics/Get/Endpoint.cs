using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Statistics.Get;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s =>
        {
            s.Summary = "Statistics for an election round";
        });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        string sql = """
        -- get observer stats
        SELECT
            COUNT(MO."Id") FILTER (
                WHERE
                    MO."Status" = 'Active'
                    AND U."Status" = 'Active'
            ) "ActiveObservers",
            COUNT(MO."Id") FILTER (
                WHERE
                    MO."Status" = 'Pending'
                    OR U."Status" = 'Pending'
            ) "PendingObservers",
            COUNT(MO."Id") FILTER (
                WHERE
                    MO."Status" = 'Suspended'
                    OR U."Status" = 'Suspended'
            ) "SuspendedObservers"
        FROM
            "MonitoringObservers" MO
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
            INNER JOIN "AspNetUsers" U ON MO."ObserverId" = U."Id"
        WHERE
            MN."ElectionRoundId" = @electionRoundId
            AND MN."NgoId" = @ngoId;
            
        -- get active observers (minimum 1 interaction)
        WITH
            MONITORING_OBSERVERS AS (
                SELECT
                    MO."Id"
                FROM
                    "MonitoringObservers" MO
                    JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    JOIN "AspNetUsers" U ON MO."ObserverId" = U."Id"
                WHERE
                    MN."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
            ),
            ACTIVE_OBSERVERS AS (
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "FormSubmissions" FS
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    FS."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "QuickReports" QR
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    QR."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "Attachments" A
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = A."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    A."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "Notes" N
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = N."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    N."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "PollingStationInformation" PSI
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    PSI."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
            )
        SELECT
            COUNT(DISTINCT MO."Id") AS "NumberOfObserversOnTheField"
        FROM
            MONITORING_OBSERVERS MO
            JOIN ACTIVE_OBSERVERS AO ON MO."Id" = AO."MonitoringObserverId";
            
        -- get count of total polling stations
        SELECT
            COUNT("Id") "NumberOfPollingStations"
        FROM
            "PollingStations"
        WHERE
            "ElectionRoundId" = @electionRoundId
            
        -- get visited polling stations
        WITH
            MONITORING_OBSERVER AS (
                SELECT
                    MO."Id"
                FROM
                    "MonitoringObservers" MO
                    JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    JOIN "AspNetUsers" U ON MO."ObserverId" = U."Id"
                WHERE
                    MN."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
            ),
            POLLING_STATION_VISITS AS (
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "FormSubmissions" FS
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    FS."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "QuickReports" QR
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    QR."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "Attachments" A
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = A."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    A."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "Notes" N
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = N."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    N."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "PollingStationInformation" PSI
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    PSI."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
            )
        SELECT
            COUNT(DISTINCT PSV."PollingStationId") AS "NumberOfVisitedPollingStations"
        FROM
            MONITORING_OBSERVER MO
            JOIN POLLING_STATION_VISITS PSV ON MO."Id" = PSV."MonitoringObserverId";
        
        -- count total minutes observing
        SELECT
            COALESCE(SUM(COALESCE(PSI."MinutesMonitoring", 0)), 0) "MinutesMonitoring"
        FROM
            "MonitoringObservers" MO
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
            INNER JOIN "AspNetUsers" U ON MO."ObserverId" = U."Id"
            INNER JOIN "PollingStationInformation" PSI ON PSI."MonitoringObserverId" = MO."Id"
        WHERE
            MN."ElectionRoundId" = @electionRoundId
            AND MN."NgoId" = @ngoId;
            
            
        -- read hourly histogram
        WITH
            TIME_SERIES AS (
                SELECT
                    GENERATE_SERIES(
                        DATE_TRUNC(
                            'hour',
                            TIMEZONE ('utc', NOW()) + INTERVAL '-12 hour'
                        ),
                        DATE_TRUNC('hour', TIMEZONE ('utc', NOW())),
                        '1 hour'
                    ) AS "Bucket"
            ),
            FS_CTE AS (
                SELECT
                    TS."Bucket",
                    COUNT(FS."Id") AS "FormsSubmitted",
                    SUM(FS."NumberOfFlaggedAnswers") AS "NumberOfQuestionsAnswered",
                    SUM(FS."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers"
                FROM
                    "FormSubmissions" FS
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    LEFT JOIN TIME_SERIES TS ON COALESCE(FS."LastModifiedOn", FS."CreatedOn") >= TS."Bucket"
                    AND COALESCE(FS."LastModifiedOn", FS."CreatedOn") < TS."Bucket" + INTERVAL '1 hour'
                WHERE
                    FS."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                GROUP BY
                    TS."Bucket"
            ),
            QR_CTE AS (
                SELECT
                    TS."Bucket",
                    COUNT(QR."Id") AS "QuickReportsSubmitted"
                FROM
                    "QuickReports" QR
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    LEFT JOIN TIME_SERIES TS ON COALESCE(QR."LastModifiedOn", QR."CreatedOn") >= TS."Bucket"
                    AND COALESCE(QR."LastModifiedOn", QR."CreatedOn") < TS."Bucket" + INTERVAL '1 hour'
                WHERE
                    QR."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                GROUP BY
                    TS."Bucket"
            )
        SELECT
            TS."Bucket",
            COALESCE(FS."FormsSubmitted", 0) AS "FormsSubmitted",
            COALESCE(FS."NumberOfQuestionsAnswered", 0) AS "NumberOfQuestionsAnswered",
            COALESCE(FS."NumberOfFlaggedAnswers", 0) AS "NumberOfFlaggedAnswers",
            COALESCE(QR."QuickReportsSubmitted", 0) AS "QuickReportsSubmitted"
        FROM
            TIME_SERIES TS
            LEFT JOIN FS_CTE FS ON FS."Bucket" = TS."Bucket"
            LEFT JOIN QR_CTE QR ON QR."Bucket" = TS."Bucket"
        ORDER BY
            TS."Bucket";
        """;

        var queryArgs = new { electionRoundId = req.ElectionRoundId, ngoId = req.NgoId };

        ObserversStats observersStats;
        int numberOfObserversOnTheField;
        int totalNumberOfPollingStations;
        int numberOfVisitedPollingStations;
        decimal minutesMonitoring;
        List<BucketView> histogram = [];

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using (var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs))
            {
                observersStats = multi.ReadSingle<ObserversStats>();
                numberOfObserversOnTheField = multi.ReadSingle<int>();
                totalNumberOfPollingStations = multi.ReadSingle<int>();
                numberOfVisitedPollingStations = multi.ReadSingle<int>();
                minutesMonitoring = multi.ReadSingle<decimal>();
                histogram = multi.Read<BucketView>().ToList();
            }
        }

        return new Response
        {
            ObserversStats = observersStats,
            NumberOfObserversOnTheField = numberOfObserversOnTheField,
            PollingStationsStats = new()
            {
                TotalNumberOfPollingStations = totalNumberOfPollingStations,
                NumberOfVisitedPollingStations = numberOfVisitedPollingStations
            },
            MinutesMonitoring = minutesMonitoring,
            FormsHistogram = histogram.Select(x => new HistogramPoint(x.Bucket, x.FormsSubmitted)).ToArray(),
            QuestionsHistogram = histogram.Select(x => new HistogramPoint(x.Bucket, x.NumberOfQuestionsAnswered)).ToArray(),
            FlaggedAnswersHistogram = histogram.Select(x => new HistogramPoint(x.Bucket, x.NumberOfFlaggedAnswers)).ToArray(),
            QuickReportsHistogram = histogram.Select(x => new HistogramPoint(x.Bucket, x.QuickReportsSubmitted)).ToArray(),
        };
    }
}
