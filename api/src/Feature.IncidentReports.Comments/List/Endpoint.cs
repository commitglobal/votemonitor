using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.IncidentReports.Comments.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-comments"));
        Summary(s => { s.Summary = "Lists comments for an incident report."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  SELECT
                      c."Id",
                      c."ElectionRoundId",
                      c."IncidentReportId",
                      c."Text",
                      c."CreatedBy",
                      COALESCE(u."DisplayName", '') AS "CreatedByName",
                      c."CreatedOn" AS "CreatedAt",
                      c."LastModifiedOn" AS "LastModifiedAt"
                  FROM "IncidentReportComments" c
                  INNER JOIN "IncidentReports" ir ON ir."Id" = c."IncidentReportId"
                  INNER JOIN "MonitoringObservers" mo ON mo."Id" = ir."MonitoringObserverId"
                  INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                  LEFT JOIN "AspNetUsers" u ON u."Id" = c."CreatedBy"
                  WHERE c."ElectionRoundId" = @electionRoundId
                    AND c."IncidentReportId" = @incidentReportId
                    AND mn."NgoId" = @ngoId
                    AND mn."ElectionRoundId" = @electionRoundId
                  ORDER BY c."CreatedOn";
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
