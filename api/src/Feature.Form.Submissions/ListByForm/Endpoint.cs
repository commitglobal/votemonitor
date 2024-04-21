using Authorization.Policies;
using Dapper;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.ListByForm;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Response>
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
            select f.""Id"" as ""FormId"",
                   f.""Code"" as ""FormCode"",
                   f.""FormType"" as ""FormType"",
                   count(distinct fs.""Id"") ""NumberOfSubmissions"",
                   sum(fs.""NumberOfFlaggedAnswers"") ""NumberOfFlaggedAnswers"",
                   count(distinct n.""Id"") ""NumberOfNotes"",
                   count(distinct a.""Id"") ""NumberOfMediaFiles""
            from ""Forms"" f
            inner join ""MonitoringNgos"" mn on mn.""Id"" = f.""MonitoringNgoId""
            inner join ""FormSubmissions"" fs on fs.""FormId"" = f.""Id""
            left JOIN ""Notes"" n on n.""FormId"" = f.""Id""
            left JOIN ""Attachments"" a on a.""FormId"" = f.""Id""
            WHERE f.""ElectionRoundId"" = @electionRoundId
                AND mn.""NgoId"" = @ngoId
            group by f.""Id"",
                     f.""Code"",
                     f.""FormType"";
        ";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId
        };

        var aggregatedFormOverviews = await context.Connection.QueryAsync<AggregatedFormOverview>(sql, queryArgs);

        return new Response() { AggregatedForms = aggregatedFormOverviews.ToList() };
    }
}
