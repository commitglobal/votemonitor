using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.QuickReports.Comments.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("quick-report-comments"));
        Summary(s => { s.Summary = "Lists comments for a quick report."; });

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
                  FROM "GetQuickReportComments"(@electionRoundId, @ngoId, @quickReportId)
                  ORDER BY "CreatedAt";
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            quickReportId = req.QuickReportId,
            ngoId = req.NgoId
        };

        List<QuickReportCommentModel> comments;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            comments = (await dbConnection.QueryAsync<QuickReportCommentModel>(sql, queryArgs)).ToList();
        }

        return TypedResults.Ok(new Response { Comments = comments });
    }
}
