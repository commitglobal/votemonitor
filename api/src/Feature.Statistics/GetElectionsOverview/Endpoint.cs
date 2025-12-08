using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Statistics.GetElectionsOverview;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/statistics/overview");
        AllowAnonymous();
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s =>
        {
            s.Summary = "Statistics for an list of election round";
        });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var statisticsQuery = """
            -- number of monitoring observers with active account
            SELECT
                COUNT(*)
            FROM
                "MonitoringObservers" MO
                INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
            WHERE
                "ElectionRoundId" = ANY (@electionRoundIds)
                AND U."Status" = 'Active'
                AND MO."Status" = 'Active';
            ------------------------------
            
            -- number of ngos with active account
            SELECT
                COUNT(*)
            FROM
                "MonitoringNgos"
            WHERE
                "ElectionRoundId" = ANY (@electionRoundIds)
                AND "Status" = 'Active';
            ------------------------------
            
            -- total number of polling stations
            SELECT
                COUNT(*)
            FROM
                "PollingStations"
            WHERE
                "ElectionRoundId" = ANY (@electionRoundIds);
            ------------------------------
            
            -- visited stations stats
            SELECT
                COUNT(DISTINCT T."PollingStationId") AS "NumberOfVisitedPollingStations",
                COUNT(DISTINCT PS."Level1") AS "NumberOfLevel1Covered",
                COUNT(DISTINCT CONCAT(PS."Level1", '-', PS."Level2")) AS "NumberOfLevel2Covered",
                COUNT(
                    DISTINCT CONCAT(PS."Level1", '-', PS."Level2", '-', PS."Level3")
                ) AS "NumberOfLevel3Covered",
                COUNT(
                    DISTINCT CONCAT(
                        PS."Level1",
                        '-',
                        PS."Level2",
                        '-',
                        PS."Level3",
                        '-',
                        PS."Level4"
                    )
                ) AS "NumberOfLevel4Covered",
                COUNT(
                    DISTINCT CONCAT(
                        PS."Level1",
                        '-',
                        PS."Level2",
                        '-',
                        PS."Level3",
                        '-',
                        PS."Level4",
                        '-',
                        PS."Level5"
                    )
                ) AS "NumberOfLevel5Covered"
            FROM
                (
                    SELECT
                        FS."PollingStationId"
                    FROM
                        "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE
                        FS."ElectionRoundId" = ANY (@electionRoundIds)
                        AND F."Status" <> 'Drafted'
                        AND FS."NumberOfQuestionsAnswered" > 0
                    UNION
                    SELECT
                        "PollingStationId"
                    FROM
                        "PollingStationInformation"
                    WHERE
                        "ElectionRoundId" = ANY (@electionRoundIds)
                        AND "NumberOfQuestionsAnswered" > 0
                ) AS T
                INNER JOIN "PollingStations" PS ON T."PollingStationId" = PS."Id";
            -----------------------------
            
            -- number of form submissions
            SELECT
                (
                    SELECT
                        COUNT(1)
                    FROM
                        "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE
                        FS."ElectionRoundId" = ANY (@electionRoundIds)
                        AND FS."NumberOfQuestionsAnswered" > 0
                        AND F."Status" <> 'Drafted'
                ) + (
                    SELECT
                        COUNT(1)
                    FROM
                        "PollingStationInformation"
                    WHERE
                        "ElectionRoundId" = ANY (@electionRoundIds)
                        AND "NumberOfQuestionsAnswered" > 0
                ) AS "NumberOfQuestionsAnswered";
            -----------------------------
            
            -- number of questions answered
            SELECT
                (
                    SELECT
                        SUM("NumberOfQuestionsAnswered")
                    FROM
                        "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE
                        FS."ElectionRoundId" = ANY (@electionRoundIds)
                        AND FS."NumberOfQuestionsAnswered" > 0
                        AND F."Status" <> 'Drafted'
                ) + (
                    SELECT
                        SUM("NumberOfQuestionsAnswered")
                    FROM
                        "PollingStationInformation"
                    WHERE
                        "ElectionRoundId" = ANY (@electionRoundIds)
                ) AS "NumberOfFormSubmissions";
            ------------------------------
            
            SELECT
                SUM("NumberOfFlaggedAnswers")
            FROM
                "FormSubmissions" FS
                INNER JOIN "Forms" F ON F."Id" = FS."FormId"
            WHERE
                FS."ElectionRoundId" = ANY (@electionRoundIds)
                AND "NumberOfQuestionsAnswered" > 0
                AND F."Status" <> 'Drafted';
            ------------------------------
            
            -- minutes monitoring
            SELECT
                SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks"))
            FROM
                "PollingStationInformation"
            WHERE
                "ElectionRoundId" = ANY (@electionRoundIds);
            ------------------------------
            """;

        var queryArgs = new
        {
            electionRoundIds = req.ElectionRoundIds
        };

        int numberOfMonitoringObservers;
        int numberOfMonitoringNgos;
        int totalNumberOfPollingStations;
        PollingStationVisitsView pollingStationVisitsView = null!;
        int numberOfSubmittedForms;
        int numberOfAnsweredQuestions;
        int numberOfFlaggedAnswers;
        int minutesMonitoring;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(statisticsQuery, queryArgs);

            numberOfMonitoringObservers = multi.ReadSingle<int>();
            numberOfMonitoringNgos = multi.ReadSingle<int>();
            totalNumberOfPollingStations = multi.ReadSingle<int>();
            pollingStationVisitsView = multi.ReadSingle<PollingStationVisitsView>();
            numberOfSubmittedForms = multi.ReadSingle<int>();
            numberOfAnsweredQuestions = multi.ReadSingle<int>();
            numberOfFlaggedAnswers = multi.ReadSingle<int>();
            minutesMonitoring = multi.ReadSingle<int>();
        }

        return new Response
        {
            Observers = numberOfMonitoringObservers,
            Ngos = numberOfMonitoringNgos,
            PollingStations = totalNumberOfPollingStations,
            VisitedPollingStations = pollingStationVisitsView?.NumberOfVisitedPollingStations ?? 0,
            Level1Visited = pollingStationVisitsView?.NumberOfLevel1Covered ?? 0,
            Level2Visited = pollingStationVisitsView?.NumberOfLevel2Covered ?? 0,
            Level3Visited = pollingStationVisitsView?.NumberOfLevel3Covered ?? 0,
            Level4Visited = pollingStationVisitsView?.NumberOfLevel4Covered ?? 0,
            Level5Visited = pollingStationVisitsView?.NumberOfLevel5Covered ?? 0,
            StartedForms = numberOfSubmittedForms,
            QuestionsAnswered = numberOfAnsweredQuestions,
            FlaggedAnswers = numberOfFlaggedAnswers,
            MinutesMonitoring = minutesMonitoring
        };
    }
}
