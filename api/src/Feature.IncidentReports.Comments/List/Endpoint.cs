using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.IncidentReports.Comments.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-comments"));
        Summary(s => { s.Summary = "Lists comments for an incident report."; });

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
                  FROM "GetIncidentReportComments"(@electionRoundId, @ngoId, @incidentReportId)
                  ORDER BY "CreatedAt";
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            incidentReportId = req.IncidentReportId,
            ngoId = req.NgoId
        };

        List<IncidentReportCommentModel> comments;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            comments = (await dbConnection.QueryAsync<IncidentReportCommentModel>(sql, queryArgs)).ToList();
        }

        return TypedResults.Ok(new Response { Comments = comments });
    }
}
