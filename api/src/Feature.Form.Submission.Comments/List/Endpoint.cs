using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Form.Submission.Comments.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("form-submission-comments"));
        Summary(s => { s.Summary = "Lists comments for a form submission."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  SELECT
                      c."Id",
                      c."ElectionRoundId",
                      c."SubmissionId",
                      c."QuestionId",
                      c."Text",
                      c."CreatedBy",
                      COALESCE(u."DisplayName", '') AS "CreatedByName",
                      c."CreatedOn" AS "CreatedAt",
                      c."LastModifiedOn" AS "LastModifiedAt"
                  FROM "FormSubmissionComments" c
                  INNER JOIN "FormSubmissions" fs ON fs."Id" = c."SubmissionId"
                  INNER JOIN "MonitoringObservers" mo ON mo."Id" = fs."MonitoringObserverId"
                  INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                  LEFT JOIN "AspNetUsers" u ON u."Id" = c."CreatedBy"
                  WHERE c."ElectionRoundId" = @electionRoundId
                    AND c."SubmissionId" = @submissionId
                    AND mn."NgoId" = @ngoId
                    AND mn."ElectionRoundId" = @electionRoundId
                  ORDER BY c."CreatedOn";
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
