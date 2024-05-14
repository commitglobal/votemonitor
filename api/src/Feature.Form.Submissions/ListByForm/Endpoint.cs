using System.Data;
using Authorization.Policies;
using Dapper;

namespace Feature.Form.Submissions.ListByForm;

public class Endpoint(IDbConnection dbConnection) : Endpoint<Request, Response>
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
            SELECT f.""Id"" AS ""FormId"",
                   f.""Code"" AS ""FormCode"",
                   f.""FormType"" AS ""FormType"",
                   count(distinct fs.""Id"") ""NumberOfSubmissions"",
                   sum(fs.""NumberOfFlaggedAnswers"") ""NumberOfFlaggedAnswers"",
                   count(distinct n.""Id"") ""NumberOfNotes"",
                   count(distinct a.""Id"") ""NumberOfMediaFiles""
            FROM ""Forms"" f
            INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = f.""MonitoringNgoId""
            INNER JOIN ""FormSubmissions"" fs ON fs.""FormId"" = f.""Id""
            LEFT JOIN ""Notes"" n ON n.""FormId"" = f.""Id""
            LEFT JOIN ""Attachments"" a ON a.""FormId"" = f.""Id""
            WHERE f.""ElectionRoundId"" = @electionRoundId
                AND mn.""NgoId"" = @ngoId
            GROUP BY f.""Id"",
                     f.""Code"",
                     f.""FormType"";
        ";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId
        };

        var aggregatedFormOverviews = await dbConnection.QueryAsync<AggregatedFormOverview>(sql, queryArgs);

        return new Response { AggregatedForms = aggregatedFormOverviews.ToList() };
    }
}
