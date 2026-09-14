using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.CitizenReports.Comments.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("citizen-report-comments"));
        Summary(s => { s.Summary = "Lists comments for a citizen report."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT
                      c."Id",
                      c."ElectionRoundId",
                      c."CitizenReportId",
                      c."Text",
                      c."CreatedBy",
                      COALESCE(u."DisplayName", '') AS "CreatedByName",
                      c."CreatedOn" AS "CreatedAt",
                      c."LastModifiedOn" AS "LastModifiedAt"
                  FROM "CitizenReportComments" c
                  INNER JOIN "CitizenReports" cr ON cr."Id" = c."CitizenReportId"
                  INNER JOIN "ElectionRounds" er ON er."Id" = cr."ElectionRoundId"
                  INNER JOIN "MonitoringNgos" mn ON mn."Id" = er."MonitoringNgoForCitizenReportingId"
                  LEFT JOIN "AspNetUsers" u ON u."Id" = c."CreatedBy"
                  WHERE c."ElectionRoundId" = @electionRoundId
                    AND c."CitizenReportId" = @citizenReportId
                    AND mn."NgoId" = @ngoId
                    AND mn."ElectionRoundId" = @electionRoundId
                  ORDER BY c."CreatedOn";
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId, citizenReportId = req.CitizenReportId, ngoId = req.NgoId
        };

        List<CitizenReportCommentModel> comments;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            comments = (await dbConnection.QueryAsync<CitizenReportCommentModel>(sql, queryArgs)).ToList();
        }

        return TypedResults.Ok(new Response { Comments = comments });
    }
}
