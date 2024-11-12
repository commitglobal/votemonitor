using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.CitizenReports.GetFilters;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports:filters");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Summary(s => { s.Summary = "Filter options for citizen reports."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT MIN(COALESCE(CR."LastModifiedOn", CR."CreatedOn")) AS "FirstSubmissionTimestamp",
                         MAX(COALESCE(CR."LastModifiedOn", CR."CreatedOn")) AS "LastSubmissionTimestamp"
                  FROM "CitizenReports" CR
                           INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                           INNER JOIN "MonitoringNgos" MN ON ER."MonitoringNgoForCitizenReportingId" = MN."Id"
                  WHERE CR."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId;
                  
                  -- =====================================================================================
                  SELECT DISTINCT F."Id" AS                       "FormId",
                                  F."Name" ->> F."DefaultLanguage" "FormName",
                                  F."Code"                        "FormCode"
                  FROM "CitizenReports" CR
                           INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                           INNER JOIN "MonitoringNgos" MN ON ER."MonitoringNgoForCitizenReportingId" = MN."Id"
                           INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                  WHERE CR."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId
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
            FormFilterOptions = formFilterOptions
        });
    }
}