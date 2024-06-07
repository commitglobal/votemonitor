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
                AND MO."Status" = 'Active'
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
            
            -- number of visited polling stations
            SELECT
                COUNT(DISTINCT "PollingStationId") AS "NumberOfVisitedPollingStations"
            FROM
                (
                    SELECT
                        FS."PollingStationId"
                    FROM
                        "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE
                        FS."ElectionRoundId" = ANY (@electionRoundIds)
                        AND F."Status" = 'Published'
                        AND FS."NumberOfQuestionsAnswered" > 0
                    UNION
                    SELECT
                        "PollingStationId"
                    FROM
                        "PollingStationInformation"
                    WHERE
                        "ElectionRoundId" = ANY (@electionRoundIds)
                        AND "NumberOfQuestionsAnswered" > 0
                ) AS T;
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
                        AND F."Status" = 'Published'
                ) + (
                    SELECT
                        COUNT(1)
                    FROM
                        "PollingStationInformation"
                    WHERE
                        "ElectionRoundId" = ANY (@electionRoundIds)
                        AND "NumberOfQuestionsAnswered" > 0
                ) AS "NumberOfFormSubmissions";
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
                        AND F."Status" = 'Published'
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
                AND F."Status" = 'Published';
            ------------------------------
            
            -- minutes monitoring
            SELECT
                SUM(COALESCE("MinutesMonitoring", 0))
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

        long numberOfMonitoringObservers;
        long numberOfMonitoringNgos;
        long totalNumberOfPollingStations;
        long numberOfVisitedPollingStations;
        long numberOfSubmittedForms;
        long numberOfAnsweredQuestions;
        long numberOfFlaggedAnswers;
        long minutesMonitoring;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(statisticsQuery, queryArgs);

            numberOfMonitoringObservers = multi.ReadSingle<long>();
            numberOfMonitoringNgos = multi.ReadSingle<long>();
            totalNumberOfPollingStations = multi.ReadSingle<long>();
            numberOfVisitedPollingStations = multi.ReadSingle<long>();
            numberOfSubmittedForms = multi.ReadSingle<long>();
            numberOfAnsweredQuestions = multi.ReadSingle<long>();
            numberOfFlaggedAnswers = multi.ReadSingle<long>();
            minutesMonitoring = multi.ReadSingle<long>();
        }

        return new Response()
        {
            Observers = numberOfMonitoringObservers,
            Ngos = numberOfMonitoringNgos,
            PollingStations = totalNumberOfPollingStations,
            VisitedPollingStations = numberOfVisitedPollingStations,
            StartedForms = numberOfSubmittedForms,
            QuestionsAnswered = numberOfAnsweredQuestions,
            FlaggedAnswers = numberOfFlaggedAnswers,
            MinutesMonitoring = minutesMonitoring
        };
    }
}
