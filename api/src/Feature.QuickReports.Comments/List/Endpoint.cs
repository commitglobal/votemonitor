using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.QuickReports.Comments.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("quick-report-comments"));
        Summary(s => { s.Summary = "Lists comments for a quick report."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  SELECT
                      c."Id",
                      c."ElectionRoundId",
                      c."QuickReportId",
                      c."Text",
                      c."CreatedBy",
                      COALESCE(u."DisplayName", '') AS "CreatedByName",
                      c."CreatedOn" AS "CreatedAt",
                      c."LastModifiedOn" AS "LastModifiedAt"
                  FROM "QuickReportComments" c
                  INNER JOIN "QuickReports" qr ON qr."Id" = c."QuickReportId"
                  INNER JOIN "MonitoringObservers" mo ON mo."Id" = qr."MonitoringObserverId"
                  INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                  LEFT JOIN "AspNetUsers" u ON u."Id" = c."CreatedBy"
                  WHERE c."ElectionRoundId" = @electionRoundId
                    AND c."QuickReportId" = @quickReportId
                    AND mn."NgoId" = @ngoId
                    AND mn."ElectionRoundId" = @electionRoundId
                  ORDER BY c."CreatedOn";
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
