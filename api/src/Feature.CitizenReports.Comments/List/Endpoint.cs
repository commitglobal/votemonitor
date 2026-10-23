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

        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT *
                  FROM "GetCitizenReportComments"(@electionRoundId, @ngoId, @citizenReportId)
                  ORDER BY "CreatedAt";
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
