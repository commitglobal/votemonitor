using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.QuickReports.GetFilters;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports:filters");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s => { s.Summary = "Filter options for quick reports."; });

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
                  SELECT MIN(QR."LastUpdatedAt") AS "FirstSubmissionTimestamp",
                         MAX(QR."LastUpdatedAt") AS "LastSubmissionTimestamp"
                  FROM "QuickReports" QR
                  INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                    ON QR."MonitoringObserverId" = MO."MonitoringObserverId"
                  WHERE QR."ElectionRoundId" = @electionRoundId;

                  -- =====================================================================================
                  SELECT DISTINCT
                    MO."MonitoringObserverId",
                    MO."DisplayName",
                    MO."Email",
                    MO."AccountStatus"
                  FROM "QuickReports" QR
                  INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                    ON QR."MonitoringObserverId" = MO."MonitoringObserverId"
                  WHERE QR."ElectionRoundId" = @electionRoundId
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString()
        };

        SubmissionsTimestampsFilterOptions timestampFilterOptions;
        List<SubmissionsObserverFilterOption> observerFilterOptions;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            timestampFilterOptions = multi.Read<SubmissionsTimestampsFilterOptions>().Single();
            observerFilterOptions = multi.Read<SubmissionsObserverFilterOption>().ToList();
        }

        return TypedResults.Ok(new Response
        {
            TimestampsFilterOptions = timestampFilterOptions,
            ObserverFilterOptions = observerFilterOptions
        });
    }
}
