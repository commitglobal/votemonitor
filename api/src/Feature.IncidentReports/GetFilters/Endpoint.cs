using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.IncidentReports.GetFilters;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports:filters");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Summary(s => { s.Summary = "Filter options for incident reports."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT MIN(COALESCE(IR."LastModifiedOn", IR."CreatedOn")) AS "FirstSubmissionTimestamp",
                         MAX(COALESCE(IR."LastModifiedOn", IR."CreatedOn")) AS "LastSubmissionTimestamp"
                  FROM "IncidentReports" IR
                           INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                           INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                  WHERE IR."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId;

                  -- =====================================================================================

                  SELECT DISTINCT F."Id" AS                       "FormId",
                                  F."Name" ->> F."DefaultLanguage" "FormName",
                                  F."Code"                        "FormCode"
                  FROM "IncidentReports" IR
                           INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                           INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                           INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                  WHERE IR."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
        };

        SubmissionsTimestampsFilterOptions timestampFilterOptions;
        List<SubmissionsFormFilterOption> formFilterOptions;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            timestampFilterOptions = multi.Read<SubmissionsTimestampsFilterOptions>().Single();
            formFilterOptions = multi.Read<SubmissionsFormFilterOption>().ToList();
        }

        return TypedResults.Ok(new Response
        {
            TimestampsFilterOptions = timestampFilterOptions,
            FormFilterOptions = formFilterOptions,
        });
    }
}