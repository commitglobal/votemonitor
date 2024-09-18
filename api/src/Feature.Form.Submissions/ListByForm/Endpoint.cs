using Authorization.Policies;
using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Form.Submissions.ListByForm;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byForm");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x =>
        {
            x.Summary = "Form submissions aggregated by observer";
        });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
        SELECT
            F."Id" AS "FormId",
            'PSI' AS "FormCode",
            'PSI' AS "FormType",
            COUNT(DISTINCT PSI."Id") "NumberOfSubmissions",
            SUM(PSI."NumberOfFlaggedAnswers") "NumberOfFlaggedAnswers",
            0 AS "NumberOfMediaFiles",
            0 AS "NumberOfNotes"
        FROM
            "PollingStationInformationForms" F
            INNER JOIN "PollingStationInformation" PSI ON PSI."ElectionRoundId" = F."ElectionRoundId"
            INNER JOIN "PollingStations" PS ON PSI."PollingStationId" = PS."Id"
            INNER JOIN "MonitoringObservers" mo ON mo."Id" = PSI."MonitoringObserverId"
        
        WHERE
            F."ElectionRoundId" = @electionRoundId
            AND (@level1 IS NULL OR ps."Level1" = @level1)
            AND (@level2 IS NULL OR ps."Level2" = @level2)
            AND (@level3 IS NULL OR ps."Level3" = @level3)
            AND (@level4 IS NULL OR ps."Level4" = @level4)
            AND (@level5 IS NULL OR ps."Level5" = @level5)
            AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
            AND (@hasFlaggedAnswers is NULL OR ("NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = false) OR ("NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = true))
            AND (@followUpStatus is NULL OR "FollowUpStatus" = @followUpStatus)
            AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
        
        GROUP BY
            F."Id"
        UNION ALL
        SELECT
            F."Id" AS "FormId",
            F."Code" AS "FormCode",
            F."FormType" AS "FormType",
            COUNT(DISTINCT FS."Id") "NumberOfSubmissions",
            SUM(FS."NumberOfFlaggedAnswers") "NumberOfFlaggedAnswers",
            (
                SELECT
                    COUNT(1)
                FROM
                    "Attachments"
                WHERE
                    "FormId" = F."Id"
                    AND "IsCompleted" = TRUE AND "IsDeleted" = FALSE
            ) AS "NumberOfMediaFiles",
            (
                SELECT
                    COUNT(1)
                FROM
                    "Notes"
                WHERE
                    "FormId" = F."Id"
            ) AS "NumberOfNotes"
        FROM
            "Forms" F
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
            INNER JOIN "FormSubmissions" FS ON FS."FormId" = F."Id"
            INNER JOIN "PollingStations" ps ON ps."Id" = FS."PollingStationId"
            INNER JOIN "MonitoringObservers" mo ON mo."Id" = FS."MonitoringObserverId"
        
        WHERE
            F."ElectionRoundId" = @electionRoundId
            AND MN."NgoId" = @ngoId
            AND (@level1 IS NULL OR ps."Level1" = @level1)
            AND (@level2 IS NULL OR ps."Level2" = @level2)
            AND (@level3 IS NULL OR ps."Level3" = @level3)
            AND (@level4 IS NULL OR ps."Level4" = @level4)
            AND (@level5 IS NULL OR ps."Level5" = @level5)
            AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
            AND (@hasFlaggedAnswers is NULL OR ("NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = false) OR ("NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = true))
            AND (@followUpStatus is NULL OR "FollowUpStatus" = @followUpStatus)
            AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
        
        GROUP BY
            F."Id",
            F."Code",
            F."FormType";
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            followUpStatus = req.FollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
        };

        IEnumerable<AggregatedFormOverview> aggregatedFormOverviews = [];

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregatedFormOverviews = await dbConnection.QueryAsync<AggregatedFormOverview>(sql, queryArgs);
        }

        return new Response { AggregatedForms = aggregatedFormOverviews.ToList() };
    }
}
