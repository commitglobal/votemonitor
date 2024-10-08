using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Statistics.GetNgoAdminStatistics;

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
            "ElectionRoundId" = @electionRoundId;
            
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
                    AND QR."PollingStationId" IS NOT NULL
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
                UNION ALL
                SELECT
                    "MonitoringObserverId",
                    "PollingStationId"
                FROM
                    "IncidentReports" IR
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                WHERE
                    IR."ElectionRoundId" = @electionRoundId
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
        SELECT DATE_TRUNC('hour', TIMEZONE('utc', COALESCE(FS."LastModifiedOn", FS."CreatedOn")))::TIMESTAMPTZ AS "Bucket",
               COUNT(1) AS "FormsSubmitted",
               SUM("NumberOfFlaggedAnswers") AS "NumberOfQuestionsAnswered",
               SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers"
        FROM "FormSubmissions" FS
                 INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                 INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
        WHERE FS."ElectionRoundId" = @electionRoundId
          AND MN."NgoId" = @ngoId
        GROUP BY 1;
        
        SELECT DATE_TRUNC('hour', TIMEZONE('utc', COALESCE(QR."LastModifiedOn", QR."CreatedOn")))::TIMESTAMPTZ "Bucket",
               COUNT(1) "Value"
        FROM "QuickReports" QR
                 INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                 INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
        WHERE QR."ElectionRoundId" = @electionRoundId
          AND MN."NgoId" = @ngoId
        GROUP BY 1;

        SELECT DATE_TRUNC('hour', TIMEZONE('utc', COALESCE(CR."LastModifiedOn", CR."CreatedOn")))::TIMESTAMPTZ "Bucket",
               COUNT(1) "Value"
        FROM "CitizenReports" CR
                 INNER JOIN PUBLIC."ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                 INNER JOIN PUBLIC."MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
        WHERE CR."ElectionRoundId" = '5e55767e-6d9f-45e5-951f-1643f2153400'
          AND MN."NgoId" = @ngoId
        GROUP BY 1;
        
        SELECT DATE_TRUNC('hour', TIMEZONE('utc', COALESCE(IR."LastModifiedOn", IR."CreatedOn")))::TIMESTAMPTZ "Bucket",
               COUNT(1) "Value"
        FROM "IncidentReports" IR
                 INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                 INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
        WHERE IR."ElectionRoundId" = @electionRoundId
          AND MN."NgoId" = @ngoId
        GROUP BY 1;
        """;

        var queryArgs = new { electionRoundId = req.ElectionRoundId, ngoId = req.NgoId };

        ObserversStats observersStats;
        int numberOfObserversOnTheField;
        int totalNumberOfPollingStations;
        int numberOfVisitedPollingStations;
        decimal minutesMonitoring;
        List<FormSubmissionsHistogramPoint> formSubmissionsHistogram = [];
        List<HistogramPoint> quickReportsHistogram = [];
        List<HistogramPoint> incidentReportsHistogram = [];
        List<HistogramPoint> citizenReportsHistogram = [];

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using (var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs))
            {
                observersStats = multi.ReadSingle<ObserversStats>();
                numberOfObserversOnTheField = multi.ReadSingle<int>();
                totalNumberOfPollingStations = multi.ReadSingle<int>();
                numberOfVisitedPollingStations = multi.ReadSingle<int>();
                minutesMonitoring = multi.ReadSingle<decimal>();
                formSubmissionsHistogram = multi.Read<FormSubmissionsHistogramPoint>().ToList();
                quickReportsHistogram = multi.Read<HistogramPoint>().ToList();
                incidentReportsHistogram = multi.Read<HistogramPoint>().ToList();
                citizenReportsHistogram = multi.Read<HistogramPoint>().ToList();
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
            FormsHistogram = formSubmissionsHistogram.Select(x => new HistogramPoint
            {
                Bucket   = x.Bucket,
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
            CitizenReportsHistogram = citizenReportsHistogram.ToArray(),
        };
    }
}
