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
                  SELECT MIN(IR."LastUpdatedAt") AS "FirstSubmissionTimestamp",
                         MAX(IR."LastUpdatedAt") AS "LastSubmissionTimestamp"
                  FROM "IncidentReports" IR
                  INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                    ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                  WHERE IR."ElectionRoundId" = @electionRoundId;

                  -- =====================================================================================
                  SELECT DISTINCT
                    F."Id" AS "FormId",
                    F."Name" ->> F."DefaultLanguage" AS "FormName",
                    F."Code" AS "FormCode"
                  FROM "IncidentReports" IR
                  INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                  INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                    ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                  WHERE IR."ElectionRoundId" = @electionRoundId;

                  -- =====================================================================================
                  SELECT DISTINCT
                    MO."MonitoringObserverId",
                    MO."DisplayName",
                    MO."Email",
                    MO."AccountStatus"
                  FROM "IncidentReports" IR
                  INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                    ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                  WHERE IR."ElectionRoundId" = @electionRoundId
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString()
        };

        SubmissionsTimestampsFilterOptions timestampFilterOptions;
        List<SubmissionsFormFilterOption> formFilterOptions;
        List<SubmissionsObserverFilterOption> observerFilterOptions;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            timestampFilterOptions = multi.Read<SubmissionsTimestampsFilterOptions>().Single();
            formFilterOptions = multi.Read<SubmissionsFormFilterOption>().ToList();
            observerFilterOptions = multi.Read<SubmissionsObserverFilterOption>().ToList();
        }

        return TypedResults.Ok(new Response
        {
            TimestampsFilterOptions = timestampFilterOptions,
            FormFilterOptions = formFilterOptions,
            ObserverFilterOptions = observerFilterOptions
        });
    }
}
