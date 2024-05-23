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
        var sql = @"
        SELECT
            F.""Id"" AS ""FormId"",
            F.""Code"" AS ""FormCode"",
            F.""FormType"" AS ""FormType"",
            COUNT(DISTINCT FS.""Id"") ""NumberOfSubmissions"",
            SUM(FS.""NumberOfFlaggedAnswers"") ""NumberOfFlaggedAnswers"",
            (
                SELECT
                    COUNT(1)
                FROM
                    ""Attachments""
                WHERE
                    ""FormId"" = F.""Id""
            ) AS ""NumberOfMediaFiles"",
            (
                SELECT
                    COUNT(1)
                FROM
                    ""Notes""
                WHERE
                    ""FormId"" = F.""Id""
            ) AS ""NumberOfNotes""
        FROM
            ""Forms"" F
            INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = F.""MonitoringNgoId""
            INNER JOIN ""FormSubmissions"" FS ON FS.""FormId"" = F.""Id""
        WHERE
            F.""ElectionRoundId"" = @electionRoundId
            AND MN.""NgoId"" = @ngoId
        GROUP BY
            F.""Id"",
            F.""Code"",
            F.""FormType"";
        ";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId
        };

        IEnumerable<AggregatedFormOverview> aggregatedFormOverviews = [];

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregatedFormOverviews = await dbConnection.QueryAsync<AggregatedFormOverview>(sql, queryArgs);
        }

        return new Response { AggregatedForms = aggregatedFormOverviews.ToList() };
    }
}
