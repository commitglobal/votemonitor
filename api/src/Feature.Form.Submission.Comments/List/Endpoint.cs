using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Form.Submission.Comments.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("form-submission-comments"));
        Summary(s => { s.Summary = "Lists comments for a form submission."; });

        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT *
                  FROM "GetFormSubmissionComments"(@electionRoundId, @ngoId, @submissionId)
                  ORDER BY "CreatedAt";
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            submissionId = req.SubmissionId,
            ngoId = req.NgoId
        };

        List<FormSubmissionCommentModel> comments;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            comments = (await dbConnection.QueryAsync<FormSubmissionCommentModel>(sql, queryArgs)).ToList();
        }

        return TypedResults.Ok(new Response { Comments = comments });
    }
}
